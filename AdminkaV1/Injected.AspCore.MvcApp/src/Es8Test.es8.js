export default class Es8Test {
    constructor() {
    }

    async run() {
        function GetMessage() {
            return new Promise(resolve => {
                setTimeout(() => {
                    const object = { x: 'es8', y: 1 };
                    const values = Object.values(object);
                    resolve(values[0]);
                }, 100);
            });
        };

        const message = await GetMessage();
        //console.log(`This is ${message}`, );
    }
}