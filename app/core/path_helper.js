(function (global, app) {

    /**
     * Class to help create the path for a resource in this application
     * A partial implementation
     */
    app.qwDefine(
        class PathHelper {

            /**
             * Get the path for the controller javascript
             */
            static getControllerPath(name) {
                return PathHelper.getPath(`${app.Config.PATH.CONTROLLER}/${name}/${name}_controller.js`);
            }

            /**
             * Get the path for the controller javascript
             */
            static getModelPath(controller_name) {
                return PathHelper.getPath(`${app.Config.PATH.CONTROLLER}/${name}/${name}_model.js`);
            }

            /**
             * Get the html template path
             * @param {string} name
             */
            static getHtmlPath(name) {
                return PathHelper.getPath(`${app.Config.PATH.CONTROLLER}/${name}/${name}.html`);
            }

            /**
             * Get the path for the controller javascript
             */
            static getSharedControllerPath(name) {
                return PathHelper.getPath(`${app.Config.PATH.SHARED}/${name}/${name}_controller.js`);
            }

            /**
             * Get the path for the controller javascript
             */
            static getSharedModelPath(controller_name) {
                return PathHelper.getPath(`${app.Config.PATH.SHARED}/${name}/${name}_model.js`);
            }

            /**
             * Get the html template path
             * @param {string} name
             */
            static getSharedHtmlPath(name) {
                return PathHelper.getPath(`${app.Config.PATH.SHARED}/${name}/${name}.html`);
            }

            /**
             * Get the file from the path.
             * It is equivalent to a config file or constants defined globally
             */
            static getPath(fs) {
                return new URL(fs, `${global.location.protocol}//${global.location.host}`).href;
            }
        }
    );
})(window, window.qwapp);
