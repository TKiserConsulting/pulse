/* tslint:disable */
/* eslint-disable */
import { CheckinData } from './checkin-data';
import { EmoticonData } from './emoticon-data';
import { SessionListItemDto } from './session-list-item-dto';
export interface SessionReportResultDto {
  checkins: Array<CheckinData>;
  emoticons: Array<EmoticonData>;
  intervalMinutes: number;
  session: SessionListItemDto;
}
