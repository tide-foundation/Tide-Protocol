// Tide Protocol - Infrastructure for the Personal Data economy
// Copyright (C) 2019 Tide Foundation Ltd
//
// This program is free software and is subject to the terms of
// the Tide Community Open Source License as published by the
// Tide Foundation Limited. You may modify it and redistribute
// it in accordance with and subject to the terms of that License.
// This program is distributed WITHOUT WARRANTY of any kind,
// including without any implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE.
// See the Tide Community Open Source License for more details.
// You should have received a copy of the Tide Community Open
// Source License along with this program.
// If not, see https://tide.org/licenses_tcosl-1-0-en

import EthnicityClassification from "./EthnicityClassification";
import AgeClassification from "./AgeClassification";

/** @type {{isType: (type:string) => boolean; }[]} */
const classifications = [EthnicityClassification, AgeClassification];

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