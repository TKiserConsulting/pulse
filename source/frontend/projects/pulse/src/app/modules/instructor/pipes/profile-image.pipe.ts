import { Pipe, PipeTransform } from '@angular/core';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { ProfilesService } from '@app/api/services';
import { map, Observable, switchMap } from 'rxjs';

@Pipe({
    name: 'profileImage',
})
export class ProfileImagePipe implements PipeTransform {
    constructor(
        private profilesService: ProfilesService,
        private sanitizer: DomSanitizer
    ) {}

    transform(
        url$: Observable<string | null>,
        imageSize?: string
    ): Observable<SafeUrl | string> {
        return url$.pipe(
            switchMap((url) => {
                const image$ = imageSize?.startsWith('small')
                    ? this.profilesService.loadSmallImage()
                    : this.profilesService.loadImage();
                return image$;
            }),
            map((image: any) => {
                return image
                    ? this.sanitizer.bypassSecurityTrustUrl(
                          URL.createObjectURL(image)
                      )
                    : 'assets/images/account_circle_black_24dp.svg';
            })
        );
    }
}
