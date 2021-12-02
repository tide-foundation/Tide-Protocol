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

export default class TideConfig {
  static get threshold() { return 3; }

  /**
   * Calculates whether the number of errors affects the interpolation 
   * @param {number} errorCount 
   * @param {number} shareCount 
   * @returns 
   */
  static isError(errorCount, shareCount) {
    return errorCount > (shareCount - TideConfig.threshold);
  }
}
