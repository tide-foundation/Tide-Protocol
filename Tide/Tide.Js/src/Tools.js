// @ts-check

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