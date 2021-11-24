/* tslint:disable */
/* eslint-disable */
import { CheckinData } from './checkin-data';
import { ClassListItemDto } from './class-list-item-dto';
import { EmoticonData } from './emoticon-data';
import { SessionListItemDto } from './session-list-item-dto';
export interface SessionReportResultDto {
  checkins: Array<CheckinData>;
  class: ClassListItemDto;
  emoticons: Array<EmoticonData>;
  intervalMinutes: number;
  session: SessionListItemDto;
}
