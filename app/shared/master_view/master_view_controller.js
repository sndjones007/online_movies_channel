(function(global, app) {

  /**
   * Class which renders the master template
   */
  app.qwDefine(
    class MasterViewController extends BaseController {

      /**
       * Constructor with the route of the controller
       * @param {string} route
       */
      constructor(route) {
          super(route, true);
          app.AutoBind.execute(this);
      }

      /**
       * The main / default template to load
       * @param {*} params
       */
      async index(params) {
          let template = await this.templatePromise;
          let modelClass = app[app.NameHelper.getSharedModelName(this.route.controller)];

          this.render(template, modelClass);
          console.log("Rendered Cover Page");
      }
    }
  );

})(window, window.qwapp);
