import EthnicityClassification from "./EthnicityClassification";

/** @type {{isType: (type:string) => boolean; }[]} */
const classifications = [EthnicityClassification];

/**
 * @param {import('../MetaField').default} field
 * @param {string} type
 * @returns {{fieldType: 'input'|'select'; classify: () => string; options: () => import('../MetaField').MetaOption[]; }}
 */
export default function classificator(field, type) {
    const any = classifications.find(itm => itm.isType(type));

    if (!any) throw new Error(`invalid classificator ${type}`);
    
    return new any(field, type);
}

export class EmptyClassification {
    /**@type {'input'|'select'}*/
    get fieldType() { return 'input'; }

    /** @returns {string} */
    classify() { return null; }

    /** @returns { import('../MetaField').MetaOption[] } */
    options() { return []; }
}