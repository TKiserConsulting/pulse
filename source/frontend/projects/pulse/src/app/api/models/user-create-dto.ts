/* tslint:disable */
/* eslint-disable */
import { UserRole } from './user-role';
export interface UserCreateDto {
  email: string;
  password: string;
  role: UserRole;
}
