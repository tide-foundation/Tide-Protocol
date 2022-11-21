
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
        if (!obj.To || !obj.From || !obj.EncryptedData)
            throw Error(`The JSON is not in the correct format: ${data}`);

        const to = obj.To;
        const from = obj.From;
        const encryptedData = obj.EncryptedData;
  
        return new GenShardShareResponse(to, from, encryptedData);
    }

}