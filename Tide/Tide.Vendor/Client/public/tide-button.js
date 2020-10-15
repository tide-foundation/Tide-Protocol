var Tide;Tide=(()=>{"use strict";var t={426:(t,e,n)=>{n.d(e,{Z:()=>r});var i=n(645),o=n.n(i)()(!1);o.push([t.id,'#tide button{position:relative;background:#4184f3;cursor:pointer;width:340px;height:66px;color:white;border:0px;border-radius:2px;z-index:1;outline:none}#tide button:hover #logo-back{background:#316cca}#tide button #logo-back{position:absolute;background:#397ae4;width:88px;height:66px;left:0px;border-radius:3px;z-index:2;-webkit-transition:all 0.3s;transition:all 0.3s;-webkit-transition-timing-function:ease-in-out;transition-timing-function:ease-in-out}#tide button #logo-back.processing{background:#ff6c00}#tide button #tide-content{display:-webkit-box;display:-ms-flexbox;display:flex;-webkit-box-orient:horizontal;-webkit-box-direction:normal;-ms-flex-direction:row;flex-direction:row;-webkit-box-pack:space-evenly;-ms-flex-pack:space-evenly;justify-content:space-evenly;-webkit-box-align:center;-ms-flex-align:center;align-items:center}#tide button #tide-content #tide-logo{width:25px;z-index:4}#tide button #tide-content #tide-logo.processing{-webkit-animation:rotation 2s infinite linear;animation:rotation 2s infinite linear}@-webkit-keyframes rotation{from{-webkit-transform:rotate(0deg);transform:rotate(0deg)}to{-webkit-transform:rotate(359deg);transform:rotate(359deg)}}@keyframes rotation{from{-webkit-transform:rotate(0deg);transform:rotate(0deg)}to{-webkit-transform:rotate(359deg);transform:rotate(359deg)}}#tide button #tide-content #tide-title{width:70%;font-size:19px;font-family:"DM Sans", sans-serif;font-weight:bold}#tide button #tide-status{font-family:"DM Sans", sans-serif;position:absolute;right:5px;bottom:2px;display:-webkit-box;display:-ms-flexbox;display:flex;-webkit-box-orient:horizontal;-webkit-box-direction:normal;-ms-flex-direction:row;flex-direction:row}#tide button #tide-status #status-text{-webkit-transform:translate(0px, -1px);transform:translate(0px, -1px);font-size:10px}\n',""]);const r=o},645:t=>{t.exports=function(t){var e=[];return e.toString=function(){return this.map((function(e){var n=function(t,e){var n,i,o,r=t[1]||"",a=t[3];if(!a)return r;if(e&&"function"==typeof btoa){var s=(n=a,i=btoa(unescape(encodeURIComponent(JSON.stringify(n)))),o="sourceMappingURL=data:application/json;charset=utf-8;base64,".concat(i),"/*# ".concat(o," */")),c=a.sources.map((function(t){return"/*# sourceURL=".concat(a.sourceRoot||"").concat(t," */")}));return[r].concat(c).concat([s]).join("\n")}return[r].join("\n")}(e,t);return e[2]?"@media ".concat(e[2]," {").concat(n,"}"):n})).join("")},e.i=function(t,n,i){"string"==typeof t&&(t=[[null,t,""]]);var o={};if(i)for(var r=0;r<this.length;r++){var a=this[r][0];null!=a&&(o[a]=!0)}for(var s=0;s<t.length;s++){var c=[].concat(t[s]);i&&o[c[0]]||(n&&(c[2]?c[2]="".concat(n," and ").concat(c[2]):c[2]=n),e.push(c))}},e}},379:(t,e,n)=>{var i,o=function(){var t={};return function(e){if(void 0===t[e]){var n=document.querySelector(e);if(window.HTMLIFrameElement&&n instanceof window.HTMLIFrameElement)try{n=n.contentDocument.head}catch(t){n=null}t[e]=n}return t[e]}}(),r=[];function a(t){for(var e=-1,n=0;n<r.length;n++)if(r[n].identifier===t){e=n;break}return e}function s(t,e){for(var n={},i=[],o=0;o<t.length;o++){var s=t[o],c=e.base?s[0]+e.base:s[0],d=n[c]||0,u="".concat(c," ").concat(d);n[c]=d+1;var l=a(u),f={css:s[1],media:s[2],sourceMap:s[3]};-1!==l?(r[l].references++,r[l].updater(f)):r.push({identifier:u,updater:b(f,e),references:1}),i.push(u)}return i}function c(t){var e=document.createElement("style"),i=t.attributes||{};if(void 0===i.nonce){var r=n.nc;r&&(i.nonce=r)}if(Object.keys(i).forEach((function(t){e.setAttribute(t,i[t])})),"function"==typeof t.insert)t.insert(e);else{var a=o(t.insert||"head");if(!a)throw new Error("Couldn't find a style target. This probably means that the value for the 'insert' parameter is invalid.");a.appendChild(e)}return e}var d,u=(d=[],function(t,e){return d[t]=e,d.filter(Boolean).join("\n")});function l(t,e,n,i){var o=n?"":i.media?"@media ".concat(i.media," {").concat(i.css,"}"):i.css;if(t.styleSheet)t.styleSheet.cssText=u(e,o);else{var r=document.createTextNode(o),a=t.childNodes;a[e]&&t.removeChild(a[e]),a.length?t.insertBefore(r,a[e]):t.appendChild(r)}}function f(t,e,n){var i=n.css,o=n.media,r=n.sourceMap;if(o?t.setAttribute("media",o):t.removeAttribute("media"),r&&"undefined"!=typeof btoa&&(i+="\n/*# sourceMappingURL=data:application/json;base64,".concat(btoa(unescape(encodeURIComponent(JSON.stringify(r))))," */")),t.styleSheet)t.styleSheet.cssText=i;else{for(;t.firstChild;)t.removeChild(t.firstChild);t.appendChild(document.createTextNode(i))}}var w=null,p=0;function b(t,e){var n,i,o;if(e.singleton){var r=p++;n=w||(w=c(e)),i=l.bind(null,n,r,!1),o=l.bind(null,n,r,!0)}else n=c(e),i=f.bind(null,n,e),o=function(){!function(t){if(null===t.parentNode)return!1;t.parentNode.removeChild(t)}(n)};return i(t),function(e){if(e){if(e.css===t.css&&e.media===t.media&&e.sourceMap===t.sourceMap)return;i(t=e)}else o()}}t.exports=function(t,e){(e=e||{}).singleton||"boolean"==typeof e.singleton||(e.singleton=(void 0===i&&(i=Boolean(window&&document&&document.all&&!window.atob)),i));var n=s(t=t||[],e);return function(t){if(t=t||[],"[object Array]"===Object.prototype.toString.call(t)){for(var i=0;i<n.length;i++){var o=a(n[i]);r[o].references--}for(var c=s(t,e),d=0;d<n.length;d++){var u=a(n[d]);0===r[u].references&&(r[u].updater(),r.splice(u,1))}n=c}}}},897:(t,e,n)=>{n.r(e),n.d(e,{init:()=>g});var i=n(379),o=n.n(i),r=n(426);o()(r.Z,{insert:"head",singleton:!1});var a,s,c,d,u,l,f,w,p="<style>"+(r.Z.locals||{})+'</style> <button><div id="tide-content"><div id="logo-back"></div><img id="tide-logo" alt="Tide Authentication Button" src="data:image/svg+xml;base64,PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiPz4NCjwhLS0gR2VuZXJhdG9yOiBBZG9iZSBJbGx1c3RyYXRvciAyNC4yLjAsIFNWRyBFeHBvcnQgUGx1Zy1JbiAuIFNWRyBWZXJzaW9uOiA2LjAwIEJ1aWxkIDApICAtLT4NCjxzdmcgdmVyc2lvbj0iMS4xIiBpZD0iTGF5ZXJfMSIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIiB4bWxuczp4bGluaz0iaHR0cDovL3d3dy53My5vcmcvMTk5OS94bGluayIgeD0iMHB4IiB5PSIwcHgiDQoJIHZpZXdCb3g9IjAgMCAxMC41IDEyIiBzdHlsZT0iZW5hYmxlLWJhY2tncm91bmQ6bmV3IDAgMCAxMC41IDEyOyIgeG1sOnNwYWNlPSJwcmVzZXJ2ZSI+DQo8c3R5bGUgdHlwZT0idGV4dC9jc3MiPg0KCS5zdDB7ZmlsbDojRkZGRkZGO30NCjwvc3R5bGU+DQo8Zz4NCgk8cGF0aCBjbGFzcz0ic3QwIiBkPSJNOC40LDQuMUw3LjIsNC44djQuNmwtMS45LDEuMWMtMC4xLDAtMC4xLDAuMS0wLjEsMC4yVjEyYzAsMCwwLDAsMC4xLDBsNS4yLTNjMCwwLDAsMCwwLTAuMVY3LjYNCgkJYzAtMC4xLTAuMS0wLjEtMC4xLTAuMUw4LjUsOC42VjQuMkM4LjUsNC4xLDguNCw0LjEsOC40LDQuMSIvPg0KCTxwYXRoIGNsYXNzPSJzdDAiIGQ9Ik01LjIsOC41TDEuMyw2LjNWMy45YzAtMC4xLDAtMC4xLTAuMS0wLjJMMC4xLDMuMWMwLDAtMC4xLDAtMC4xLDAuMXY2YzAsMC4xLDAsMC4xLDAuMSwwLjJMMS4yLDEwDQoJCWMwLjEsMCwwLjEsMCwwLjEtMC4xVjcuOEw1LDkuOWMwLjEsMCwwLjEsMCwwLjEtMC4xTDUuMiw4LjVMNS4yLDguNXoiLz4NCgk8cGF0aCBjbGFzcz0ic3QwIiBkPSJNMy4yLDQuOUw3LDIuN2wxLjgsMS4xYzAuMSwwLDAuMSwwLDAuMiwwbDEuMS0wLjZjMC4xLDAsMC4xLTAuMSwwLTAuMkw0LjksMEM0LjgsMCw0LjgsMCw0LjcsMEwzLjYsMC43DQoJCWMtMC4xLDAtMC4xLDAuMSwwLDAuMmwyLDEuMkwyLjEsNEMyLDQuMSwyLDQuMiwyLjEsNC4yTDMuMiw0Ljl6Ii8+DQo8L2c+DQo8L3N2Zz4NCg==" /><span id="tide-title">Continue with Tide</span></div><span id="tide-status"> <span id="status-text"></span></span></button>';function b(t){document.getElementById("status-text").innerHTML=t}function M(t){d.classList[t?"add":"remove"]("processing"),u.classList[t?"add":"remove"]("processing")}function g(t,e,n){w=t,f=e,l=n}window.onload=function(){return(c=document.getElementById("tide")).innerHTML=p,d=document.getElementById("tide-logo"),u=document.getElementById("logo-back"),void c.addEventListener("click",(function(){null!=(s=window.open(l,"auth","width=500, height=501,top=0,right=0"))&&(b("Awaiting login"),M(!0),a=window.setInterval((function(){s.closed&&(b("Window closed without action"),clearInterval(a),M(!1))}),100),window.addEventListener("message",(function(t){var e;"tide-onload"==t.data.type&&s.postMessage({type:"tide-init",_serverUrl:f,_homeUrl:w},l),"tide-authenticated"==t.data.type&&(e=t.data,clearInterval(a),s.close(),b("Finishing authentication"),M(!1),b("jwt: "+e.data.jwt))})))}))}}},e={};function n(i){if(e[i])return e[i].exports;var o=e[i]={id:i,exports:{}};return t[i](o,o.exports,n),o.exports}return n.n=t=>{var e=t&&t.__esModule?()=>t.default:()=>t;return n.d(e,{a:e}),e},n.d=(t,e)=>{for(var i in e)n.o(e,i)&&!n.o(t,i)&&Object.defineProperty(t,i,{enumerable:!0,get:e[i]})},n.o=(t,e)=>Object.prototype.hasOwnProperty.call(t,e),n.r=t=>{"undefined"!=typeof Symbol&&Symbol.toStringTag&&Object.defineProperty(t,Symbol.toStringTag,{value:"Module"}),Object.defineProperty(t,"__esModule",{value:!0})},n(897)})();