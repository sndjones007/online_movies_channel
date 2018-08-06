(function(global, app) {

  /**
   * The main class of this application
   */
  app.qwDefine(
    class AppMain {

      /**
       * Constructor
       */
      constructor() {
        app.AutoBind.execute(this);
      }

      /**
       * Initialize method which starts the program
       */
      init() {
        app.Log('Application loaded at ' + Date.toString());

        let program = new app.QwProgram();
        program.init().then(() => program.execute())
          .catch((err) => app.ELog(err));
      }
    }
  );
})(window, window.qwapp);
