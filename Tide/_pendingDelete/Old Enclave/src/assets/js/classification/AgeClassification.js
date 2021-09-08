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

const rx = /^age(?:\:(\w+))?$/;

export default class AgeClassification {
    /**
     * @param {string} type 
     * @returns {boolean}
     */
    static isType(type) { return rx.exec(type) !== null; }

    get fieldType() { return 'input'; }

    /**
     * @param {import('../MetaField').default} field
     * @param {string} type
     */
    constructor(field, type) {
        this.field = field;
        /** @type {'general'|'labour'|'standard'} */
        this.type = rx.exec(type)[1] || 'general';
        if (!['general', 'labour', 'standard'].some(itm => itm === this.type))
            throw new Error(`Invalid age classification type '${this.type}'`);
    }

    /** @returns {string} */
    classify() {
        if (this.field.isEncrypted) return null;
        
        if (!this.field.value) return '';

        const age = ageCalculator(new Date(this.field.value))

        if (this.type === 'standard') return standardClass(age);
        if (this.type === 'general') return generalClass(age);
        if (this.type === 'labour') return labourClass(age);
        
        throw new Error(`Invalid age classification type '${this.type}'`);
    }

    /** @returns { import('../MetaField').MetaOption[] } */
    options() { return []; }
}

/**
 * @param {Date} birthday 
 * @returns {number}
 */
function ageCalculator(birthday) {
    var ageDate = new Date(Date.now() - birthday);
    return Math.abs(ageDate.getUTCFullYear() - 1970);
}

/**
 * @param {number} age 
 * @returns {string}
 */
function generalClass(age) {
    if (age <= 4) return '0-4';
    if (age >= 5 && age <= 9) return '5-9';
    if (age >= 10 && age <= 14) return '10-14';
    if (age >= 15 && age <= 19) return '15-19';
    if (age >= 20 && age <= 24) return '20-24';
    if (age >= 25 && age <= 29) return '25-29';
    if (age >= 30 && age <= 34) return '30-34';
    if (age >= 35 && age <= 39) return '35-39';
    if (age >= 40 && age <= 44) return '40-44';
    if (age >= 45 && age <= 49) return '45-49';
    if (age >= 50 && age <= 54) return '50-54';
    if (age >= 55 && age <= 59) return '55-59';
    if (age >= 60 && age <= 64) return '60-64';
    if (age >= 65 && age <= 69) return '65-69';
    if (age >= 70 && age <= 74) return '70-74';
    if (age >= 75 && age <= 79) return '75-79';
    if (age >= 80 && age <= 84) return '80-84';
    if (age >= 85 && age <= 89) return '85-89';
    if (age >= 90 && age <= 94) return '90-94';
    if (age >= 95 && age <= 99) return '95-99';
    if (age >= 100 && age <= 104) return '100-104';
    if (age >= 105) return '105 and over';

    throw new Error('Error in code: invalid age classification');
}

/**
 * @param {number} age 
 * @returns {string}
 */
function labourClass(age) {
    if (age < 15) return '';
    if (age >= 15 && age <= 19) return '15-19';
    if (age >= 20 && age <= 24) return '20-24';
    if (age >= 25 && age <= 29) return '25-29';
    if (age >= 30 && age <= 34) return '30-34';
    if (age >= 35 && age <= 39) return '35-39';
    if (age >= 40 && age <= 44) return '40-44';
    if (age >= 45 && age <= 49) return '45-49';
    if (age >= 50 && age <= 54) return '50-54';
    if (age >= 55 && age <= 59) return '55-59';
    if (age >= 60 && age <= 64) return '60-64';
    if (age >= 65 && age <= 69) return '65-69';
    if (age >= 70 && age <= 74) return '70-74';
    if (age >= 75 && age <= 79) return '75-79';
    if (age >= 80 && age <= 84) return '80-84';
    if (age >= 85) return '85 and over';

    throw new Error('Error in code: invalid age classification');
}


/**
 * @param {number} age 
 * @returns {string}
 */
function standardClass(age) {
    if (age >= 0 && age <= 4) return '0-4';
    if (age >= 5 && age <= 14) return '5-14';
    if (age >= 15 && age <= 24) return '15-24';
    if (age >= 25 && age <= 34) return '25-34';
    if (age >= 35 && age <= 44) return '35-44';
    if (age >= 45 && age <= 54) return '45-54';
    if (age >= 55 && age <= 64) return '55-64';
    if (age >= 65 && age <= 74) return '65-74';
    if (age >= 75 && age <= 84) return '75-84';
    if (age >= 85 && age <= 94) return '85-94';
    if (age >= 95 && age <= 104) return '95-104';
    if (age >= 105) return '105 and over';

    throw new Error('Error in code: invalid age classification');
}