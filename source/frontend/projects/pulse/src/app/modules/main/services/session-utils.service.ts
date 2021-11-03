import { Injectable } from '@angular/core';
import { CheckinType } from '@app/api/models/checkin-type';
import { SessionCheckinDetailsDto } from '@app/api/models/session-checkin-details-dto';

@Injectable({
    providedIn: 'root',
})
export class SessionUtilsService {
    constructor() {}

    public getCheckinTitle(
        checkin: SessionCheckinDetailsDto | CheckinType | null | undefined
    ) {
        if (!checkin) {
            return '';
        }

        const chekinType = checkin.hasOwnProperty('checkinType')
            ? (checkin as SessionCheckinDetailsDto).checkinType
            : checkin;

        return chekinType == CheckinType.WrapUp ? 'Wrap Up' : 'Check In';
    }

    // public colorToHexString(color: number) {
    //     return (
    //         '#' +
    //         (
    //             '000000' +
    //             (
    //                 ((color & 0xff) << 16) +
    //                 (color & 0xff00) +
    //                 ((color >> 16) & 0xff)
    //             ).toString(16)
    //         ).slice(-6)
    //     );
    // }

    public colorToHexString(color: number) {
        var hex = Number(color).toString(16);
        if (hex.length < 2) {
            hex = '0' + hex;
        }
        return '#' + hex;
    }

    public toRgba(value: number): string {
        value >>>= 0;
        var b = value & 0xff,
            g = (value & 0xff00) >>> 8,
            r = (value & 0xff0000) >>> 16,
            a = ((value & 0xff000000) >>> 24) / 255;
        return 'rgba(' + [r, g, b, a || 1].join(',') + ')';
    }

    public toRgbObject(value: number): { r: number; g: number; b: number } {
        value >>>= 0;
        var b = value & 0xff,
            g = (value & 0xff00) >>> 8,
            r = (value & 0xff0000) >>> 16;
        return {
            r,
            g,
            b,
        };
    }

    public rgbToNumber(rgb: { r: number; g: number; b: number }) {
        return (rgb.r << 16) + (rgb.g << 8) + rgb.b;
    }
}
