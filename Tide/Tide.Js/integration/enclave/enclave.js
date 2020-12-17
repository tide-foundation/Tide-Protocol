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

import C25519Key from "cryptide/src/c25519Key";
import MetaField from "../../src/MetaField";

(function() {
    var model = { rendered: false, encrypted: false, fields: [] };
    let source = null;
    const key = C25519Key.generate();
    
    window.addEventListener('message', (e) => {
        if (e.data.type !== 'modify' && e.data.type !== 'create')
            return;
        
        model.encrypted = e.data.type === 'modify';
        model.fields = MetaField.fromModel(e.data.model, model.encrypted);
        model.rendered = true;
        source = e.source;
    }, false);
    
    var app = new Vue({
        el: '#app',
        data: model,
        methods: {
            save: function() {
                if (!source || !source.postMessage) return;
                
                if (!this.encrypted) this.encrypt();
                source.postMessage({type: 'save', model: MetaField.buildModel(model.fields)});
            },
            cancel: function() {
                if (source && source.postMessage)
                    source.postMessage({type: 'cancel'});
                },
            encrypt: function() {
                for (const field of model.fields) {
                    field.encrypt(key);
                }
                model.encrypted = !model.encrypted;
            },
            decrypt: function() {
                for (const field of model.fields) {
                    field.decrypt(key);
                }
                model.encrypted = !model.encrypted;
            }
        }
    })    
})();
