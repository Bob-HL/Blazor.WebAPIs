import { IJSInvokable } from './Interfaces';

class GeoLocation {
    getCurrentPosition = (options: IPositionOptions): Promise<any> => {
        this.ensurePositionOptions(options);

        return new Promise((resolve, reject) => {
            const sucess = p => {
                const position = this.wrapPosition(p);
                resolve(position);
            };
            const onError = error => reject(error);

            navigator.geolocation.getCurrentPosition(sucess, onError, options);
        });
    }

    watchPosition = (options: IPositionOptions, listener: IJSInvokable): number => {
        this.ensurePositionOptions(options);

        const sucess = p => {
            const position = this.wrapPosition(p);
            listener.invokeMethod("GotPosition", position);
        };
        const onError = error => listener.invokeMethod("OnError", error);

        //  register a handler function that will be called automatically each time the position of the device changes.
        const watchId = navigator.geolocation.watchPosition(sucess, onError, options);
        return watchId;
    }

    private ensurePositionOptions = (options: IPositionOptions) => {
        if (!options) return;

        if (!options.timeout || options.timeout < 0) {
            options.timeout = Infinity;
        }

        if (options.maximumAge !== 0 && !options.maximumAge) {
            console.log('maximumAgent is null');
            options.maximumAge = Infinity;
        }

        if (options.maximumAge < 0) {
            options.maximumAge = 0;
        }
    }

    private wrapPosition = (position: GeolocationPosition) => {
        // copy the send as position.coords cannot be serialized to Blazor.
        const coords = position.coords;
        const coordinate = {
            coords: {
                accuracy: coords.accuracy,
                altitude: coords.altitude,
                altitudeAccuracy: coords.altitudeAccuracy,
                heading: coords.heading,
                latitude: coords.latitude,
                longitude: coords.longitude,
                speed: coords.speed
            },
            timestamp: position.timestamp
        };
        return coordinate;
    }
}

interface IPositionOptions {
    enableHighAccuracy: boolean;
    timeout: number;
    maximumAge: number;
}

window['__cbw_geo'] = new GeoLocation();

if (window['__cbw_js_']) {
    window['__cbw_js_'].setLoadedFlag('Geolocation');
}
