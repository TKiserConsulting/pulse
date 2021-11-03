export function trackProcessing(processingVar: string = 'processing') {
    return (target: any, key: any, descriptor: any) => {
        if (descriptor === undefined) {
            descriptor = Object.getOwnPropertyDescriptor(target, key);
        }
        const originalMethod = descriptor.value;

        // eslint-disable-next-line func-names
        descriptor.value = function (...args: any[]) {
            let result: any;
            try {
                this[processingVar] = true;

                result = originalMethod.apply(this, args);

                if (result && result.finally) {
                    result = result.finally(() => {
                        this[processingVar] = false;
                    });
                } else if (!result.finally) {
                    this[processingVar] = false;
                }
            } catch (err) {
                this[processingVar] = false;
                throw err;
            }

            return result;
        };

        return descriptor;
    };
}
