(function(global, app) {

  /**
   * Base Class for all controllers
   */
  app.qwDefine(
    class BaseController {

      /**
       * Constructor
       * @param {string} route
       */
      constructor(route, shared) {
        this.route = route;
        this.elRoot = document.getElementsByTagName(app.Config.BOOTSTRAP_TAG)[0];

        this.templatePromise = app.LocalLoad.htmlFile(
          (shared) ? app.PathHelper.getSharedHtmlPath(this.route.controller) :
            app.PathHelper.getHtmlPath(this.route.controller));

        app.AutoBind.execute(this);
      }

      /**
       * Render the page
       * @param {string} template
       * @param {string} modelClass
       */
      render(template, modelClass) {
        if (modelClass) {
          let modelObj = new modelClass();
          this.elRoot.innerHTML = Mustache.render(template, modelObj);
        } else {
          this.elRoot.innerHTML = template;
        }

        app.Log(`Successfull render of ${template}`);
      }
    }
  );

})(window, window.qwapp);
