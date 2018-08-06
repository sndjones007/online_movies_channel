(function(global, app) {

    /**
     * Class which represents the cover page controller
     */
    app.qwDefine(
        class CoverPageController extends app.BaseController {

            /**
             * Constructor with the route of the controller
             * @param {string} route 
             */
            constructor(route) {
                super(route);
                app.AutoBind.execute(this);
            }
        
            /**
             * The main / default template to load
             * @param {*} params 
             */
            async index(params) {
                let template = await this.templatePromise;
                let modelClass = app[app.NameHelper.getModelName(this.route.controller)];

                this.render(template, modelClass);
                console.log("Rendered Cover Page");
            }
        }
    );

})(window, window.qwapp);