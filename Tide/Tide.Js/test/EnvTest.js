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


export default class EnvTest {
    static get numOrks() {
        return parseInt(process.env.TEST_ORK_NUM || 3);
    }

    static get orkUrls() {
        const mask = process.env.TEST_ORK_MASK_URL || 'http://localhost:500{0}';
        return [...Array(EnvTest.numOrks)].map((_, i) => mask.replace(/\{0\}/g, i + 1));
    }
  
    static get vendorUrl() {
        return process.env.TEST_VENDOR_URL || 'http://localhost:6001';
    }
}