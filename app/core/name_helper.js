(function(global, app) {

    /**
     * Class which helps with the naming conventions in this program
     */
    app.qwDefine(
        class NameHelper {

            /**
             * Convert the controller name present in the route data file to
             * camel case
             * @param {string} name
             */
            static getControllerName(name) {
                NameHelper._innerNaming(name, "Controller");
            }

            /**
             * Convert the controller name present in the route data file to
             * camel case for model
             * @param {string} name
             */
            static getModelName(name) {
                NameHelper._innerNaming(name, "Model");
            }

            /**
             * Convert the shared controller name present in the route data file to
             * camel case
             * @param {string} name
             */
            static getSharedControllerName(name) {
                NameHelper._innerNaming(name, "Controller");
            }

            /**
             * Replace the first character to upper case
             * @param {string} name
             */
            static _capitalizeFirstLetter(splitArr) {
                for(let i = 0; i < splitArr.length; ++i)
                    splitArr[i] = splitArr[i][0].toUpperCase() + splitArr[i].slice(1);
            }

            /**
             * Inner function which provides a name for the class
             */
            static _innerNaming(name, suffix) {
                let splitArr = name.split("_");
                NameHelper._capitalizeFirstLetter(splitArr);
                return `${splitArr.join("")}${suffix}`;
            }
        }
    );
})(window, window.qwapp);
