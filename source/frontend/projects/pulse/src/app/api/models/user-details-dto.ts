/* tslint:disable */
/* eslint-disable */
import { UserRole } from './user-role';
export interface UserDetailsDto {
  city?: null | string;
  created?: string;
  createdBy?: null | string;
  email?: null | string;
  firstName?: null | string;
  id?: string;
  lastName?: null | string;
  modified?: null | string;
  modifiedBy?: null | string;
  role?: UserRole;
  school?: null | string;
  state?: null | string;
  subject?: null | string;
}
