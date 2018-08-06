(function(global, app) {

  /**
   * Custom exceptio nclass which s will help navigate the page to
   * an error page
   */
  app.qwDefine(
    class QwError extends Error {

      /**
       * Constructor
       * @param {*} params
       */
      constructor(showErrorPage, ...params) {
        super(...params);

        // Maintains proper stack trace for where our error was thrown (only available on V8)
        if (Error.captureStackTrace) {
          Error.captureStackTrace(this, CustomError);
        }

        this.date = new Date();

        if (showErrorPage) {

        }
      }
    }
  );
})(window, window.qwapp);
