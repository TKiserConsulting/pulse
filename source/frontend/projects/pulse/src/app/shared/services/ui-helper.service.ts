import { Injectable } from '@angular/core';
import { UserRole } from '@app/api/models';

@Injectable({ providedIn: 'root' })
export class UiHelperService {
    public static EmptyOption = {
        label: '',
        value: '',
    };

    public static getRoleDisplay(role: UserRole) {
        let display;
        switch (role) {
            case UserRole.Admin:
                display = 'Admin';
                break;
            case UserRole.Instructor:
                display = 'Instructor';
                break;
            case UserRole.Student:
                display = 'Student';
                break;
            default:
                display = role;
                break;
        }
        return display;
    }

    public static getCustomerAddressDisplay(dto: {
        addressLine1: string;
        addressLine2: string;
        city: string;
        state: string;
        country: string;
    }) {
        return `${dto.addressLine1 || ''} ${dto.addressLine2 || ''} ${
            dto.city || ''
        } ${dto.state || ''} ${dto.country || ''}`;
    }

    public static capitalizeFirstLetter(s: string): string {
        return s?.charAt(0).toUpperCase() + s?.slice(1);
    }
}
