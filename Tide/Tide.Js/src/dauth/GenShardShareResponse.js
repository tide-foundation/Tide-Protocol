
export default class GenShardShareResponse {
    /**
    * @param {string} to
    * @param {string} from
    * @param {string} encryptedData
    */
    constructor(to, from, encryptedData) {
        this.to = to;
        this.from = from;
        this.encryptedData = encryptedData;

    }

    toString() { return JSON.stringify(this); }
 
    inspect() { return JSON.stringify(this); }
    
   

    /** @param {string|object} data */
    static from(data) {

        const obj = typeof data === 'string' ? JSON.parse(data) : data;
        if (!obj.to || !obj.from || !obj.encryptedData )
            throw Error(`The JSON is not in the correct format: ${data}`);

        const to = obj.to;
        const from = obj.from;
        const encryptedData = obj.encryptedData;
  
        return new GenShardShareResponse(to, from, encryptedData);
    }

}