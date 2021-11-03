/* tslint:disable */
/* eslint-disable */
import { TokenDto } from './token-dto';
import { UserDetailsDto } from './user-details-dto';
export interface RegisterResultDto {
  tokenInfo: TokenDto;
  user: UserDetailsDto;
}
