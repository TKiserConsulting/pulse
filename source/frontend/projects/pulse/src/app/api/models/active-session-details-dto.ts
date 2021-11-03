/* tslint:disable */
/* eslint-disable */
import { SessionCheckinDetailsDto } from './session-checkin-details-dto';
import { SessionDetailsDto } from './session-details-dto';
import { SessionEmoticonDetailsDto } from './session-emoticon-details-dto';
export type ActiveSessionDetailsDto = SessionDetailsDto & {
'emoticons': Array<SessionEmoticonDetailsDto>;
'activeCheckin'?: SessionCheckinDetailsDto | null;
};
