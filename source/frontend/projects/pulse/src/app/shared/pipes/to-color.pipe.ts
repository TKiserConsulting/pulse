import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'appColor' })
export class ToColorPipe implements PipeTransform {
    public transform(value: number): string {
        value >>>= 0;
        var b = value & 0xff,
            g = (value & 0xff00) >>> 8,
            r = (value & 0xff0000) >>> 16,
            a = ((value & 0xff000000) >>> 24) / 255;
        return 'rgba(' + [r, g, b, a || 1].join(',') + ')';
    }
}
