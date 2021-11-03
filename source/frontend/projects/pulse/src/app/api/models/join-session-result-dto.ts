/* tslint:disable */
/* eslint-disable */
import { TokenDto } from './token-dto';
export interface JoinSessionResultDto {
  sessionId: string;
  sessionStudentId: string;
  tokenInfo: TokenDto;
}
