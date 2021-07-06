(()=>{"use strict";var e={29:(e,t,n)=>{let o,r;n.r(t),n.d(t,{deleteDB:()=>y,openDB:()=>v,unwrap:()=>f,wrap:()=>h});const i=new WeakMap,s=new WeakMap,d=new WeakMap,a=new WeakMap,c=new WeakMap;let u={get(e,t,n){if(e instanceof IDBTransaction){if("done"===t)return s.get(e);if("objectStoreNames"===t)return e.objectStoreNames||d.get(e);if("store"===t)return n.objectStoreNames[1]?void 0:n.objectStore(n.objectStoreNames[0])}return h(e[t])},set:(e,t,n)=>(e[t]=n,!0),has:(e,t)=>e instanceof IDBTransaction&&("done"===t||"store"===t)||t in e};function l(e){return"function"==typeof e?(t=e)!==IDBDatabase.prototype.transaction||"objectStoreNames"in IDBTransaction.prototype?(r||(r=[IDBCursor.prototype.advance,IDBCursor.prototype.continue,IDBCursor.prototype.continuePrimaryKey])).includes(t)?function(...e){return t.apply(f(this),e),h(i.get(this))}:function(...e){return h(t.apply(f(this),e))}:function(e,...n){const o=t.call(f(this),e,...n);return d.set(o,e.sort?e.sort():[e]),h(o)}:(e instanceof IDBTransaction&&function(e){if(s.has(e))return;const t=new Promise(((t,n)=>{const o=()=>{e.removeEventListener("complete",r),e.removeEventListener("error",i),e.removeEventListener("abort",i)},r=()=>{t(),o()},i=()=>{n(e.error||new DOMException("AbortError","AbortError")),o()};e.addEventListener("complete",r),e.addEventListener("error",i),e.addEventListener("abort",i)}));s.set(e,t)}(e),n=e,(o||(o=[IDBDatabase,IDBObjectStore,IDBIndex,IDBCursor,IDBTransaction])).some((e=>n instanceof e))?new Proxy(e,u):e);var t,n}function h(e){if(e instanceof IDBRequest)return function(e){const t=new Promise(((t,n)=>{const o=()=>{e.removeEventListener("success",r),e.removeEventListener("error",i)},r=()=>{t(h(e.result)),o()},i=()=>{n(e.error),o()};e.addEventListener("success",r),e.addEventListener("error",i)}));return t.then((t=>{t instanceof IDBCursor&&i.set(t,e)})).catch((()=>{})),c.set(t,e),t}(e);if(a.has(e))return a.get(e);const t=l(e);return t!==e&&(a.set(e,t),c.set(t,e)),t}const f=e=>c.get(e);function v(e,t,{blocked:n,upgrade:o,blocking:r,terminated:i}={}){const s=indexedDB.open(e,t),d=h(s);return o&&s.addEventListener("upgradeneeded",(e=>{o(h(s.result),e.oldVersion,e.newVersion,h(s.transaction))})),n&&s.addEventListener("blocked",(()=>n())),d.then((e=>{i&&e.addEventListener("close",(()=>i())),r&&e.addEventListener("versionchange",(()=>r()))})).catch((()=>{})),d}function y(e,{blocked:t}={}){const n=indexedDB.deleteDatabase(e);return t&&n.addEventListener("blocked",(()=>t())),h(n).then((()=>{}))}const b=["get","getKey","getAll","getAllKeys","count"],g=["put","add","delete","clear"],p=new Map;function m(e,t){if(!(e instanceof IDBDatabase)||t in e||"string"!=typeof t)return;if(p.get(t))return p.get(t);const n=t.replace(/FromIndex$/,""),o=t!==n,r=g.includes(n);if(!(n in(o?IDBIndex:IDBObjectStore).prototype)||!r&&!b.includes(n))return;const i=async function(e,...t){const i=this.transaction(e,r?"readwrite":"readonly");let s=i.store;return o&&(s=s.index(t.shift())),(await Promise.all([s[n](...t),r&&i.done]))[0]};return p.set(t,i),i}var x;x=u,u={...x,get:(e,t,n)=>m(e,t)||x.get(e,t,n),has:(e,t)=>!!m(e,t)||x.has(e,t)}},399:function(e,t,n){var o=this&&this.__awaiter||function(e,t,n,o){return new(n||(n=Promise))((function(r,i){function s(e){try{a(o.next(e))}catch(e){i(e)}}function d(e){try{a(o.throw(e))}catch(e){i(e)}}function a(e){var t;e.done?r(e.value):(t=e.value,t instanceof n?t:new n((function(e){e(t)}))).then(s,d)}a((o=o.apply(e,t||[])).next())}))};Object.defineProperty(t,"__esModule",{value:!0});const r=n(29);class i{constructor(e,t){this.options=e,this.indexedDb=t,this.upgrade=(e,t,n,o)=>{const r=this.indexedDb.invokeMethod("GetSchema",t,n);if(r&&r.length>0){for(let t of r)if(e.objectStoreNames.contains(t.name)&&o.objectStore(t.name).keyPath!=t.keyPath&&e.deleteObjectStore(t.name),e.objectStoreNames.contains(t.name)){const e=o.objectStore(t.name),n=t.indexes,r=e.indexNames;if(r.length>0)for(let t=0;t<r.length;t++){let o=r[t];n&&n.find((e=>e.name==o))?t++:e.deleteIndex(o)}if(n)for(let t of n)r.contains(t.name)||e.createIndex(t.name,t.keyPath,{unique:t.unique})}else{var i=t.keyPath?{keyPath:t.keyPath}:{autoIncrement:t.autoIncrement};const n=e.createObjectStore(t.name,i);if(t.indexes)for(let e of t.indexes)n.createIndex(e.name,e.keyPath,{unique:e.unique})}this.options.version=n,console.log(`IndexedDB ${this.options.name} has been upgraded from version ${t} to  ${n}.`)}},this.blocked=()=>{this.indexedDb.invokeMethod("OnDbEvent",s.Blocked)},this.blocking=()=>{this.indexedDb.invokeMethod("OnDbEvent",s.Blocking)},this.terminated=()=>{this.indexedDb.invokeMethod("OnDbEvent",s.Terminated)},this.getByKey=(e,t)=>o(this,void 0,void 0,(function*(){const n=this.db.transaction(e,"readonly").objectStore(e);return yield n.get(t)})),this.getByKeyRange=(e,t,n,r,i)=>o(this,void 0,void 0,(function*(){let o=this.getQuery(t,n,r,i);const s=this.db.transaction(e,"readonly").objectStore(e);return yield s.get(o)})),this.getFromIndex=(e,t,n,r,i,s)=>o(this,void 0,void 0,(function*(){let o=this.getQuery(n,r,i,s);return yield this.db.getFromIndex(e,t,o)})),this.getAll=(e,t,n,r,i,s)=>o(this,void 0,void 0,(function*(){let o=this.getQuery(t,n,r,i);const d=this.db.transaction(e,"readonly").objectStore(e);return yield d.getAll(o,s)})),this.getQuery=(e,t,n,o)=>{let r=null,i=null!==n&&"undefined"!==n;return null!==e&&"undefined"!==e?r=i?IDBKeyRange.bound(e,n,t,o):IDBKeyRange.lowerBound(e,t):i&&(r=IDBKeyRange.upperBound(n,o)),r},this.getAllFromIndex=(e,t,n,r,i,s,d)=>o(this,void 0,void 0,(function*(){let o=this.getQuery(n,r,i,s);return this.db.getAllFromIndex(e,t,o,d)})),this.getAllByIndexValue=(e,t,n)=>o(this,void 0,void 0,(function*(){return this.db.getAllFromIndex(e,t,n)})),this.getAllKeysByIndexValue=(e,t,n)=>o(this,void 0,void 0,(function*(){return this.db.getAllKeysFromIndex(e,t,n)})),this.getAllKeys=(e,t,n,r,i,s)=>o(this,void 0,void 0,(function*(){let o=this.getQuery(t,n,r,i);const d=this.db.transaction(e,"readonly").objectStore(e);return yield d.getAllKeys(o,s)})),this.getAllKeysFromIndex=(e,t,n,r,i,s,d)=>o(this,void 0,void 0,(function*(){let o=this.getQuery(n,r,i,s);return this.db.getAllKeysFromIndex(e,t,o,d)})),this.getAllIndexValues=(e,t)=>o(this,void 0,void 0,(function*(){let n=yield this.db.transaction(e).store.index(t).openKeyCursor(),o=[];for(;n;){let e=n.key;o.indexOf(e)<0&&o.push(e),n=yield n.continue()}return o})),this.count=e=>o(this,void 0,void 0,(function*(){return this.db.count(e)})),this.put=(e,t)=>o(this,void 0,void 0,(function*(){const n=this.db.transaction(e,"readwrite");if(Array.isArray(t)){const e=n.store;for(const n of t)e.put(n)}else n.store.put(t);yield n.done})),this.delete=(e,t)=>o(this,void 0,void 0,(function*(){const n=this.db.transaction(e,"readwrite"),o=n.objectStore(e);if(Array.isArray(t))for(const e of t)o.delete(e);else o.delete(t);return n.complete})),this.clear=e=>o(this,void 0,void 0,(function*(){const t=this.db.transaction(e,"readwrite");return yield t.objectStore(e).clear(),t.complete}))}}var s;!function(e){e[e.Blocked=0]="Blocked",e[e.Blocking=1]="Blocking",e[e.Terminated=2]="Terminated"}(s||(s={})),window.__cbw_idb=new class{initIndexedDb(e,t){return o(this,void 0,void 0,(function*(){const n=new i(e,t);let o=yield r.openDB(e.name,e.version,n);o&&(n.db=o,window[e.indexedDbAgentName]=n)}))}},window.__cbw_js_&&window.__cbw_js_.setLoadedFlag("IndexedDb")}},t={};function n(o){var r=t[o];if(void 0!==r)return r.exports;var i=t[o]={exports:{}};return e[o].call(i.exports,i,i.exports,n),i.exports}n.d=(e,t)=>{for(var o in t)n.o(t,o)&&!n.o(e,o)&&Object.defineProperty(e,o,{enumerable:!0,get:t[o]})},n.o=(e,t)=>Object.prototype.hasOwnProperty.call(e,t),n.r=e=>{"undefined"!=typeof Symbol&&Symbol.toStringTag&&Object.defineProperty(e,Symbol.toStringTag,{value:"Module"}),Object.defineProperty(e,"__esModule",{value:!0})},n(399)})();