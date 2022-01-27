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

    public getByKeyRange = async (storeName: string, lowerKey: any, lowerOpen: boolean, upperKey: any, upperOpen: boolean): Promise<any> => {
        let query = this.getQuery(lowerKey, lowerOpen, upperKey, upperOpen);
        const store = this.db.transaction(storeName, 'readonly').objectStore(storeName);
        const data = await store.get(query);
        return data;
    }

    public getAllByKeys = async (storeName: string, keys: any[]): Promise<any> => {
        const store = this.db.transaction(storeName, 'readonly').objectStore(storeName);
        let data: any[] = [];

        for (const key of keys) {
            const item = await store.get(key);

            if (item) {
                data.push(item);
            }
        }

        return data;
    }

    public getFromIndex = async (storeName: string, indexName: string, lowerKey: any, lowerOpen: boolean, upperKey: any, upperOpen: boolean): Promise<any> => {
        let query = this.getQuery(lowerKey, lowerOpen, upperKey, upperOpen);
        const data = await this.db.getFromIndex(storeName, indexName, query);
        return data;
    }

    public getAll = async (storeName: string, lowerKey: any, lowerOpen: boolean, upperKey: any, upperOpen: boolean, count?: number): Promise<any> => {
        let query = this.getQuery(lowerKey, lowerOpen, upperKey, upperOpen);
        const store = this.db.transaction(storeName, 'readonly').objectStore(storeName);
        const data = await store.getAll(query, count);
        return data;
    }

    private getQuery = (lowerKey: any, lowerOpen: boolean, upperKey: any, upperOpen: boolean): IDBKeyRange | null => {
        let query: IDBKeyRange | null = null;
        let hasLowerBound = lowerKey !== null && lowerKey !== 'undefined';
        let hasUpperBound = upperKey !== null && upperKey !== 'undefined';

        if (hasLowerBound) {
            if (hasUpperBound) {
                query = IDBKeyRange.bound(lowerKey, upperKey, lowerOpen, upperOpen);
            }
            else {
                query = IDBKeyRange.lowerBound(lowerKey, lowerOpen);
            }
        }
        else if (hasUpperBound) {
            query = IDBKeyRange.upperBound(upperKey, upperOpen);
        }

        return query;
    }

    public getAllFromIndex = async (storeName: string, indexName: string, lowerKey: any, lowerOpen: boolean, upperKey: any, upperOpen: boolean, count?: number): Promise<any[]> => {
        let query = this.getQuery(lowerKey, lowerOpen, upperKey, upperOpen);
        return this.db.getAllFromIndex(storeName, indexName, query, count);
    }

    public getAllByIndexValue = async (storeName: string, indexName: string, indexValue: any): Promise<any[]> => {
        return this.db.getAllFromIndex(storeName, indexName, indexValue);
    }

    public getAllKeysByIndexValue = async (storeName: string, indexName: string, indexValue: any): Promise<any[]> => {
        return this.db.getAllKeysFromIndex(storeName, indexName, indexValue);
    }

    public getFirstKey = async (storeName: string, lowerKey: any, lowerOpen: boolean, upperKey: any, upperOpen: boolean): Promise<any[]> => {
        const store = this.db.transaction(storeName, 'readonly').objectStore(storeName);

        if (lowerKey != null && !lowerOpen && upperKey === null && !upperOpen) {
            const key = await store.getKey(lowerKey);
            return key;
        }

        let query = this.getQuery(lowerKey, lowerOpen, upperKey, upperOpen);
        const key = await store.getKey(query);
        return key;
    }

    public getAllKeys = async (storeName: string, lowerKey: any, lowerOpen: boolean, upperKey: any, upperOpen: boolean, count?: number): Promise<any[]> => {
        let query = this.getQuery(lowerKey, lowerOpen, upperKey, upperOpen);
        const store = this.db.transaction(storeName, 'readonly').objectStore(storeName);
        const keys = await store.getAllKeys(query, count);
        return keys;
    }

    public getAllKeysFromIndex = async (storeName: string, indexName: string, lowerKey: any, lowerOpen: boolean, upperKey: any, upperOpen: boolean, count?: number): Promise<any[]> => {
        let query = this.getQuery(lowerKey, lowerOpen, upperKey, upperOpen);
        const keys = this.db.getAllKeysFromIndex(storeName, indexName, query, count);
        return keys;
    }

    public getAllIndexValues = async (storeName: string, indexName: string): Promise<any[]> => {
        let cursor = await this.db.transaction(storeName).store.index(indexName).openKeyCursor();
        let keys: any[] = [];

        while (cursor) {
            let key = cursor.key;

            if (keys.indexOf(key) < 0) {
                keys.push(key);
            }

            cursor = await cursor.continue();
        }

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
