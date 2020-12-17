/**
 * Babel Starter Kit (https:
 *
 * Copyright © 2015-2016 Kriasoft, LLC. All rights reserved.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE.txt file in the root directory of this source tree.
 */

/**
 * @param {string} value
 * @param {string[]} rules
 **/
export function verify(value, rules) {
  for (const rule of rules) {
    if (rule === 'required') {
      if (!value || !value.trim()) {
        return false;
      }
      
      continue;
    }

    if (rule === 'number') {
      if (!parseInt(value)) {
        return false;
      }

      continue;
    }

    if (rule === 'date') {
      if (!Date.parse(value)) {
        return false;
      }
      
      continue;
    }

    throw new Error(`Rule '${rule}' is not defined`);
  }

  return true;
}

