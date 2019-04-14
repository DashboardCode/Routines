import WorkflowManager from './WorkflowManager.ts';

export default class Application {

    constructor(window) {
        this.window = window;
        this.document = window.document;
        this.$ = this.window.$;
        this.console = window.console;
        this.alert = window.alert;

        this.WorkflowManager = WorkflowManager;
    }

    async Es8TranspilerTest() {
        function GetMessage() {
            return new Promise(resolve => {
                setTimeout(() => {
                    const object = { x: 'es8', y: 1 };
                    const values = Object.values(object);
                    resolve(values[0]);
                }, 100);
            });
        }

        const message = await GetMessage();
        this.console.log(`This is ${message} transpiler test: OK`, );
    }
}