import { openDB, deleteDB, IDBPDatabase, TypedDOMStringList } from 'idb';
import { IJSInvokable } from './Interfaces';

class IndexedDbAgent {
    constructor(
        private options: IOpenDbOptions,
        private indexedDb: IJSInvokable
    ) {}

    //#region db

    public db: any;

    public upgrade = (db, oldVersion, newVersion, transaction) => {
        const schema = this.indexedDb.invokeMethod("GetSchema", oldVersion, newVersion);

        if (schema && schema.length > 0) {
            for (let objectStore of schema) {
                // if Key is changed, re-create the ObjectStore - data will be lost!!!
                if (db.objectStoreNames.contains(objectStore.name) && (transaction.objectStore(objectStore.name).keyPath != objectStore.keyPath)) {
                    db.deleteObjectStore(objectStore.name);
                }

                // add ObjectStore
                if (!db.objectStoreNames.contains(objectStore.name)) {
                    var params = objectStore.keyPath ? { keyPath: objectStore.keyPath } : { autoIncrement: objectStore.autoIncrement };
                    const store = db.createObjectStore(objectStore.name, params);

                    if (objectStore.indexes) {
                        for (let index of objectStore.indexes) {
                            store.createIndex(index.name, index.keyPath, { unique: index.unique });
                        }
                    }
                }
                else { // update ObjectStore
                    const store = transaction.objectStore(objectStore.name);
                    const indexes = objectStore.indexes;
                    const indexNames = <DOMStringList><unknown>store.indexNames;

                    if (indexNames.length > 0) {
                        for (let i = 0; i < indexNames.length; i++) {
                            let indexName = indexNames[i];

                            if (!indexes || !indexes.find(x => x.name == indexName)) {
                                store.deleteIndex(indexName);
                            }
                            else {
                                i++;
                            }
                        }
                    }

                    if (indexes) {
                        for (let index of indexes) {
                            if (!indexNames.contains(index.name)) {
                                store.createIndex(index.name, index.keyPath, { unique: index.unique });
                            }
                        }
                    }
                }
            }

            this.options.version = newVersion;
            console.log(`IndexedDB ${this.options.name} has been upgraded from version ${oldVersion} to  ${newVersion}.`);
        }
    }

    public blocked = () => {
        this.indexedDb.invokeMethod("OnDbEvent", DbEvent.Blocked);
    }

    public blocking = () => {
        this.indexedDb.invokeMethod("OnDbEvent", DbEvent.Blocking);
    }

    public terminated = () => {
        this.indexedDb.invokeMethod("OnDbEvent", DbEvent.Terminated);
    }

    //#endregion

    //#region ObjectStore

    public getByKey = async (storeName: string, key: any): Promise<any> => {
        const store = this.db.transaction(storeName, 'readonly').objectStore(storeName);
        const data = await store.get(key);
        return data;
    }

    public getAll = async (storeName: string): Promise<any> => {
        const store = this.db.transaction(storeName, 'readonly').objectStore(storeName);
        const data = await store.getAll();
        return data;
    }

    public getAllByIndexValue = async (storeName: string, indexName: string, indexValue: any): Promise<any[]> => {
        return this.db.getAllFromIndex(storeName, indexName, indexValue);
    }

    public getAllKeysByIndexValue = async (storeName: string, indexName: string, indexValue: any): Promise<any[]> => {
        return this.db.getAllKeysFromIndex(storeName, indexName, indexValue);
    }

    public getAllKeys = async (storeName: string): Promise<any[]> => {
        const store = this.db.transaction(storeName, 'readonly').objectStore(storeName);
        const keys = await store.getAllKeys();
        return keys;
    }

    public count = async (storeName: string): Promise<number> => {
        return this.db.count(storeName);
    }

    public put = async (storeName: string, data: any): Promise<any> => {
        const tx = this.db.transaction(storeName, 'readwrite');

        if (Array.isArray(data)) {
            const store = tx.store;

            for (const item of data) {
                store.put(item);
            }
        }
        else {
            tx.store.put(data);
        }

        await tx.done;
    }

    public delete = async (storeName: string, key: any): Promise<any> => {
        const tx = this.db.transaction(storeName, 'readwrite');
        const store = tx.objectStore(storeName);

        if (Array.isArray(key)) {
            for (const item of key) {
                store.delete(item);
            }
        }
        else {
            store.delete(key);
        }

        return tx.complete;
    }

    public clear = async (storeName: string): Promise<any> => {
        const tx = this.db.transaction(storeName, 'readwrite');
        await tx.objectStore(storeName).clear();
        return tx.complete;
    }

    //#endregion
}

class IndexedDbInitializer {
    public async initIndexedDb(options: IOpenDbOptions, indexedDb: IJSInvokable): Promise<void> {
        console.log(indexedDb);
        const agent = new IndexedDbAgent(options, indexedDb);
        let db = await openDB(options.name, options.version, agent);

        if (db) {
            agent.db = db;
            window[options.indexedDbAgentName] = agent;
        }
    }
}

interface IOpenDbOptions {
    name: string,
    version: number;
    indexedDbAgentName: string;
    schemaHash: string;
}

enum DbEvent {
    Blocked,
    Blocking,
    Terminated,
}

window["__cbw_idb"] = new IndexedDbInitializer();

if (window['__cbw_js_']) {
    window['__cbw_js_'].setLoadedFlag('IndexedDb');
}
