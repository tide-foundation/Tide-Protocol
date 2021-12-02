// @ts-check

export class ClientError extends Error {
    /**
     * @param {string} message 
     * @param {string} method 
     * @param {string} path 
     * @param {string} status 
     * @param {string} code 
     */
    constructor(message, method, path, status='', code='') {
        super(message)
        this.method = method;
        this.path = path;
        this.status = status;
        this.code = code;
    }

    toString() {
        return this.inspect();
    }
    
    inspect() {
        const code = this.status && this.code ? `${this.status}:${this.code}`
            : this.status ? this.status : this.code;

        return `[${code}] ${this.message}\n${this.method} ${this.path}`
    }
}

export class ClientErrors extends ClientError {
    /**
     * @param {string} message 
     * @param {ClientError[]} errors 
     */
    constructor(errors, message = null) {
        if (!errors || !errors.length) throw new Error("The error must not be empty");
        super(message ? message: errors[0].message, errors[0].method, errors[0].path, errors[0].status, errors[0].code);
        this.errors = errors;
    }

    inspect() {
        return this.errors.map(err => err.inspect()).join('\n\n');
    }
}

export class Errors extends Error {
    /**
     * @param {string} message 
     * @param {Error[]} errors 
     */
    constructor(errors, message = null) {
        if (!errors || !errors.length) throw new Error("The error must not be empty");
        super(message ? message: errors[0].message);
        this.errors = errors;
    }

    inspect() {
        return this.errors.map(err => `[${err.name}] '${err.message}'`).join('\n\n');
    }
}
