// @ts-check

import { processError } from "./dauth/ClientBase";
import { Errors } from "./dauth/Errors";
import TideConfig from "./TideConfig";

/** @template TValue */
export class Dictionary {
    /** @param {{[x: string]: TValue}} dic */
    constructor(dic = {}) {
        this.dic = dic;
    }

    get keys() { return Object.keys(this.dic); }

    get values() { return Object.values(this.dic); }
    
    /** @param {string} key */
    get(key) { return this.dic[key]; }

    /**
     * @param {string} key
     * @param {TValue} value
     */
    set(key, value) { this.dic[key] = value; }

    /**
     * @template TReturn
     * @param {(value: TValue, key: string, dic: {[x: string]: TValue}) => TReturn} fun
     * @returns {Dictionary<TReturn>}
     */
    map(fun) { return Dictionary.buildFrom(this.keys, key => fun(this.dic[key], key, this.dic)); }

    /**
     * @template TReturn
     * @param {(previous: TReturn, current: TValue, key: string, dic: {[x: string]: TValue}) => TReturn} fun
     * @param {TReturn} initial
     * @returns {TReturn}
     */
    reduce(fun, initial = undefined) { return this.keys.reduce((previous, key) =>
        fun(previous, this.dic[key], key, this.dic), initial); }
    
    /**
     * @template TValue
     * @param {string[]} keys
     * @param {(key: string) => TValue} callback
     * @returns {Dictionary<TValue>}
     */
    static buildFrom(keys, callback) { return new Dictionary(buildDic(keys, callback)); }
}

/** 
 * @template TValue
 * @extends {Promise<DictionaryPromise<TValue>>}
 */
export class DictionaryPromise {
    /** @param {{[x: string]: Promise<TValue>}} dic */
    constructor(dic = {}) {
        this.dic = dic;
    }

    get keys() { return Object.keys(this.dic); }

    get values() { return Object.values(this.dic); }

    get length() { return Object.keys(this.dic).length; }
    
    /** @param {string} key */
    get(key) { return this.dic[key]; }

    /**
     * @param {string} key
     * @param {Promise<TValue>} value
     */
    set(key, value) { this.dic[key] = value; }

    /**
     * @param {(value: Dictionary<TValue>) => Dictionary<TValue> | PromiseLike<Dictionary<TValue>>} callback
     * @param {(reason: Errors) => PromiseLike<never>} callbackErr
     * @returns {Promise<Dictionary<TValue>>}
     */
    async then(callback, callbackErr) {
        /** @type {Error[]} */
        const errors = []
        
        /** @type {Dictionary<TValue>} */
        const values = new Dictionary();
        
        const keys = this.keys;
        const resps = await Promise.all(this.values.map(processError));
        for (let i = 0, resp = resps[i]; i < resps.length; i++, resp = resps[i]) {
            if (resp instanceof Error) errors.push(resp);
            else values.set(keys[i], resp);
        }
        
        if (TideConfig.isError(errors.length, keys.length))
            return callbackErr(new Errors(errors));

        return callback(values);
    }

    /**
     * @param {(reason: Errors) => PromiseLike<never>} callbackErr
     * @returns {Promise<Dictionary<TValue>>}
     */
    catch(callbackErr) {
        return this.then(value => value, callbackErr);
    }

    /**
     * @template TReturn
     * @param {(value: TValue, key: string, dic: {[x: string]: Promise<TValue>}) => TReturn} fun
     * @returns {DictionaryPromise<TReturn>}
     */
    map(fun) { return DictionaryPromise.buildFrom(this.keys, key =>
        this.dic[key].then(val => fun(val, key, this.dic))); }

    /**
     * @template TReturn
     * @param {(previous: TReturn, current: TValue, key: string, dic: {[x: string]: Promise<TValue>}) => TReturn} fun
     * @param {TReturn} initial
     * @returns {Promise<TReturn>}
     */
    async reduce(fun, initial = undefined) {
        /** @type {Error[] } */
        const errors = [];
        const keys = this.keys;
        const values = this.values.map(processError);

        let previous = initial;
        while (values.length) {
            const value = await Promise.race(values);

            for (let i = 0; i < values.length; i++) {
                if (await promiseState(values[i]) === 'pending') continue;

                if (value instanceof Error) {
                    errors.push(value);
                }
                else {
                    previous = fun(previous, value, keys[i], this.dic);
                }

                values.splice(i, 1);
                keys.splice(i, 1);
                break;
            }

            if (TideConfig.isError(errors.length, this.length)) throw new Errors(errors);
        }

        return previous;
    }

    /**
     * @template TValue
     * @param {string[]} keys
     * @param {(key: string) => Promise<TValue>} callback
     * @returns {DictionaryPromise<TValue>}
     */
    static buildFrom(keys, callback) { return new DictionaryPromise(buildDic(keys, callback)); }
}


/**
 * @template TValue
 * @template TReturn
 * @param {{[x: string]: TValue}} dic
 * @param {(value: TValue, key: string, index: number) => TReturn} fun
 * @returns {{[x: string]: TReturn}}
 */
export function mapDic(dic, fun) {
    /** @type {{[x: string]: TReturn}} */
    const newDic = {};
    const keys = Object.keys(dic);
    for (let i = 0; i < keys.length; i++) {
        newDic[keys[i]]  = fun(dic[keys[i]], keys[i], i);
    }

    return newDic;
}

/**
 * @template T
 * @param {string[]} keys
 * @param {(key: string) => T} callback
 * @returns {{[x: string]: T}}
 */
function buildDic(keys, callback) {
    return (keys.reduce((dic, key) => (dic[key] = callback(key), dic), {}));
}

/** 
 * @param {Promise} p 
 * @returns {Promise<'pending'|'fulfilled'|'rejected'>}
 */
function promiseState(p) {
  const o = {};
  return Promise.race([p, o]).then(v => v === o ? "pending" : "fulfilled", () => "rejected");
}
