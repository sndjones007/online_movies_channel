/**
 * @file This file should always be included in the start html file to load the application.
 * @author S N DJones <sndjones007@gmail.com>
 * @copyright Whatever copyright you want you can add
 */

/**
 * This partial block is used to initialize the basiuc minimum types required for bootstrapping
 * and starting the application
 */
(function (global) {

    /**
     * Define the doument.currentScript propetry if not defined
     */
    document.currentScript = document.currentScript || (function () {
        var scripts = document.getElementsByTagName('script');
        return scripts[scripts.length - 1];
    })();

    /**
     * Declare the global app namespace
     */
    global.qwapp = {};
    let app = global.qwapp;

    /**
     * Read the arguments passed to the bootstrap script tag
     */
    let debugVal = document.currentScript.getAttribute('debug');

    app.Debug = (debugVal && (debugVal === "" || +debugVal > 0));

    /**
     * Define the logging base for the application
     */
    app.Log = app.Debug ? console.log.bind(console) : () => { };
    app.DLog = app.Debug ? console.debug.bind(console) : () => { };
    app.ELog = console.error.bind(console);

    /**
     * Resolves Promises sequentially
     * @param {*} funcs 
     */
    const promiseSerial = arrPromises =>
        arrPromises.reduce((promise) =>
            promise.then(result => promise.then(Array.prototype.concat.bind(result))),
                Promise.resolve([]));
    app.promiseSerial = promiseSerial;

    /**
    * A global method to define a class type. It internally merges any existing
    * same class definitions into simple type. This tries to implement the
    * concept of 'partial' in C#.
    * If no pre-existing type is found it defines the type under the app
    * namespace.
    */
    app.qwDefine = function (typeObj) {
        if (app[typeObj.name]) {
            Object.getOwnPropertyNames(typeObj)
                .filter((prop) => {
                    if (typeof typeObj[prop] == 'function') {
                        app[typeObj.name][prop] = typeObj[prop];
                    }
                });
        }
        else {
            app[typeObj.name] = typeObj;
        }
    }

    /**
     * Class used to bind the this pointer to the funtions of the class objects.
     * This is required for specifically callback and async methods.
     */
    app.qwDefine(
        class AutoBind {

            /**
             * Bind the this pointer to the method using bind() method call
             * and create a new property with the same name.
             * @param {any} thisObj
             */
            static execute(thisObj) {
                this.bindToThis(thisObj, thisObj);
                this.bindToThis(Object.getPrototypeOf(thisObj), thisObj);
            }

            /**
             * Bind this to the object
             * @param {*} thisObj 
             */
            static bindToThis(bindObj, thisObj) {
                Object.getOwnPropertyNames(bindObj)
                    .filter((prop) => {
                        if (typeof bindObj[prop] == 'function') {
                            bindObj[prop] = bindObj[prop].bind(thisObj);
                        }
                    });
            }
        }
    );

    /**
     * Class to help create the path for a resource in this application
     * A partial implementation
     */
    app.qwDefine(
        class PathHelper {

            /**
             * Get the metdata file which contains all the basic data for the data.
             * It is equivalent to a config file or constants defined globally
             */
            static getMetadataPath() {
                return new URL("metadata.json", `${global.location.protocol}//${global.location.host}`).href;
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

    /**
     * Class to load a local file
     */
    app.qwDefine(
        class LocalLoad {

            /**
             * Method to load any file (async mode)
             * @param {string} path Specifies the path of the file (Relative path or full path)
             * @param {string} contentType The type similar to the meta tag content-type
             * @param {string} responseType The response type to pass as parameter to XMLHttpRequest
             * @returns {number} Returns response data wrapped in Promise object
             */
            static anyfile(path, contentType, responseType) {
                var httpConnection = new XMLHttpRequest();
                httpConnection.open("GET", path, true);
                httpConnection.setRequestHeader("Content-Type", contentType);
                httpConnection.responseType = responseType;

                // Create the promise
                var promiseObj = new Promise((resolve, reject) => {
                    httpConnection.onreadystatechange = () => {
                        if (httpConnection.readyState === 4) {
                            if (httpConnection.status === 200) {
                                app.Log(httpConnection.response);
                                resolve(httpConnection);
                            } else {
                                app.ELog(`Error (${httpConnection.statusText}) in Loading of ${path} `);
                                throw new Error(httpConnection.statusText);
                            }
                        }
                    }
                });

                httpConnection.send(null);

                return promiseObj.then(xhr => {
                    app.Log(`Got load response of ${path}`);
                    app.Log(xhr.response);
                    return xhr.response;
                });
            }

            /**
             * Method to read Json File
             * @param {string} path 
             * @returns {number} Returns json response data wrapped in Promise object
             */
            static jsonFile(path) {
                return this.anyfile(path, "application/json", "json");
            }

            /**
             * Method to read html File
             * @param {string} path 
             * @returns {number} Returns html response data wrapped in Promise object
             */
            static htmlFile(path) {
                return this.anyfile(path, "text/html", "text");
            }
        }
    );

    /**
     * Class which helps checking, loading a script dynamically
     */
    app.qwDefine(
        class ScriptLoad {

            /**
             * Method to append and load script to the html file at the end of 
             * head tag
             * @param {string} jsrelfile 
             */
            static addScript(sfile, noversion, ignoreErr) {
                if(ScriptLoad.scriptIsLoaded(sfile)) return;

                let script = document.createElement("script");
                script.type = "application/javascript";
        
                let promiseObj = new Promise((resolve, reject) => {
                    script.onload = () => resolve();
                });
        
                document.getElementsByTagName("head")[0].appendChild(script);
                script.src = (noversion) ? sfile : ScriptLoad.getScriptSrc(sfile);
        
                return (ignoreErr) ? promiseObj.catch(err => app.ELog(err)) :
                    promiseObj;
            }

            /**
             * Method to append and load script link tag to the html file at the end
             * @param {string} sfile 
             * @param {*} ignoreErr 
             */
            static addCssLink(sfile, noversion, ignoreErr) {
                if(ScriptLoad.linkIsLoaded(sfile)) return;

                let link = document.createElement("link");
                link.rel = "stylesheet";

                let promiseObj = new Promise((resolve, reject) => {
                    link.onload = () => resolve();
                });
        
                document.getElementsByTagName("head")[0].appendChild(link);
                link.href = (noversion) ? sfile : ScriptLoad.getLinkSrc(sfile);
        
                return (ignoreErr) ? promiseObj.catch(err => app.ELog(err)) :
                    promiseObj;
            }

            /**
             * Check if the script is already loaded or not
             * @param {string} sfile 
             */
            static scriptIsLoaded(sfile) {
                return document.querySelector(`script[src^='${sfile}']`);
            }

            /**
             * Get the script pth name appended with date info for refresh
             * @param {*} filePath 
             */
            static getScriptSrc(filePath) {
                return `${filePath}?${Date.now()}`;
            }

            /**
             * Check if the script is already loaded or not
             * @param {string} sfile 
             */
            static linkIsLoaded(sfile) {
                return document.querySelector(`link[href^='${sfile}']`);
            }

            /**
             * Get the script pth name appended with date info for refresh
             * @param {*} filePath 
             */
            static getLinkSrc(filePath) {
                return `${filePath}?${Date.now()}`;
            }
        }
    );

    /**
     * Method to load config file which also acts as a constant
     */
    let onLoadConfig = async function() {
        app.Log(`Loading the config at ${typeof app.LocalLoad} ${Date.now()}`);

        return app.LocalLoad.jsonFile(app.PathHelper.getMetadataPath());
    }

    /**
     * Method to prepare the index.html file for startup on load.
     * Add scripts at runtime if the scripts are not already loaded
     */
    let onLoadBootstrap = async function() {
        app.Log(`Bootstrapping Application at ${Date.now()}`);
        
        let i = 0;

        // Even if there are no scripts to load promise should process
        // successfully
        let promises = [Promise.resolve(1)];
        let jsLoadAlways = app.Config.JS.LOAD_ALWAYS;

        for (; i < jsLoadAlways.length; ++i) {
            promises.push(app.ScriptLoad.addScript(
                app.PathHelper.getPath(jsLoadAlways[i])));
        }

        Promise.all(promises)
        .then(() => new app.AppMain().init())
        .catch((err) => app.ELog(err));
    }

    /**
     * Method to handle 'load' event
     */
    global.addEventListener("load", function(event) {
        onLoadConfig().then((jsconfig) => {
            app.Config = jsconfig;
            onLoadBootstrap();
        }).catch((err) => app.ELog(err));
    });

})(window);