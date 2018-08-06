(function (global, app) {

    /**
     * The program class which is invoked from the application class
     */
    app.qwDefine(
        class QwProgram {

            /**
             * Constructor
             */
            constructor() {
                this.router = null;
                this.scriptsjs = null;
                app.AutoBind.execute(this);
            }

            /**
             * Initialize the program. The route data is loaded.
             */
            async init() {
                let routesJsonData = await app.LocalLoad.jsonFile(
                    app.PathHelper.getPath(app.Config.ROUTE_INFO_FILE));

                app.Log("Call router to resolve");
                routesJsonData.routes.forEach(item => {
                    item.path = item.path.toLowerCase();
                    item.controller = item.controller.toLowerCase();
                    item.action = item.action.toLowerCase();
                });
                this.router = new app.Router(routesJsonData.routes);
            }

            /**
             * Execute the program
             */
            async execute() {
                this.router.map();
                await this.scriptLoader()
                    .then(() => {
                      this.invokeController(true);
                      this.invokeController();
                    })
                    .catch((err) => app.ELog(err));
            }

            /**
             * Load the page scripts related to the controller path
             */
            async scriptLoader() {
                let promises = [];

                // Add css files
                if(app.Config.CSS && app.Config.CSS.VIEW) {
                  for(let i = 0; i < app.Config.CSS.VIEW.length; ++i) {
                      promises.push(app.ScriptLoad.addCssLink(app.PathHelper.getPath(
                          app.Config.CSS.VIEW[i]), false));
                  }
                }

                // Add external libs files for views
                if(app.Config.JS && app.Config.JS.VIEW) {
                  for(let i = 0; i < app.Config.JS.VIEW.length; ++i) {
                      promises.push(app.ScriptLoad.addScript(app.PathHelper.getPath(
                          app.Config.JS.VIEW[i]), false));
                  }
                }

                if(this.router.currentRoute.controller == null)
                    throw new app.QwError(false, "No Script defined for current route");

                promises.push(app.ScriptLoad.addScript(app.PathHelper.getControllerPath(
                    this.router.currentRoute.controller)));

                if(this.router.currentRoute.model)
                    promises.push(app.ScriptLoad.addScript(app.PathHelper.getModelPath(
                        this.router.currentRoute.controller), false, true));

                return Promise.all(promises);
            }

            /**
             * After the scripts are loaded, call the controller
             */
            invokeController(shared) {
              let route = this.router.currentRoute;
              let controllerClassName = (shared) ? app.NameHelper.getSharedControllerName(
                      app.Config.TEMPLATE.MASTER_HEADER) :
                      app.NameHelper.getControllerName(this.router.currentRoute.controller);
              let controllerClass = app[controllerClassName];
              let controllerObj = new controllerClass(route);

              let routeAction = (shared)? null: route.action;
              let actionMethod = null;
              if (routeAction) {
                  actionMethod = controllerObj[routeAction];
              }
              else {
                  actionMethod = controllerObj.index;
              }

              actionMethod().then(async () => app.Log("Action completed"))
                  .catch((err) => app.ELog(err));
            }
        }
    );

})(window, window.qwapp);
