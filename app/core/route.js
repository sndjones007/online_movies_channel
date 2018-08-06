(function (global, app) {

    app.Log("Start Routing module");

    /**
     * Class which represents a Route data type
     */
    app.qwDefine(

        class Route {

            /**
             * Contructor
             * @param {any} uri
             * @param {any} controller
             * @param {any} action
             * @param {any} paramArray
             */
            constructor(uri, controller, action, paramArray) {
                this.uri = uri;
                this.controller = controller;
                this.action = action;
                this.paramArray = paramArray;
                app.AutoBindMethod.execute(this);
            }

            /**
             * Call the controller method handler
             * @param {any} params
             */
            relayToController(params) {
                this.controller[this.action](params);
            }
        }
    );

    /**
     * The class whcih handles the routing logic for the application
     */
    app.qwDefine(

        class Router {

            /**
             * Constructor
             * @param {any} routes
             */
            constructor(routes) {
                this.routes = routes;
                this.currentRoute = null;
                app.AutoBind.execute(this);
            }

            /**
             * Map the current window url to the route
             * For local browser rendering the format of the Url is:
             * <domain>?<querystring>
             * Query String: 
             * 1. ctrl = controller
             * 2. mt = method
             * 3. pm = parameters to the method
             */
            map() {
                this.UrlParts = new URL(global.location.href);
                let searchParams = this.UrlParts.searchParams;
                let controller = searchParams.get("ctrl"),
                    method = searchParams.get("mt"),
                    params = searchParams.get("pm");

                if(controller == null) {
                    this.currentRoute = this.routes[0];
                }
                else {
                    this.currentRoute = this.routes.find((item) => item.path === controller.toLowerCase());
                }

                if(this.currentRoute == null) {
                    throw new app.QwError(true, `No routing found for ${global.location.href}`);
                }
            }
        }
    );

})(window, window.qwapp);
