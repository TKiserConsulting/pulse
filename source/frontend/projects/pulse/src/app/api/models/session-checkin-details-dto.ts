/* tslint:disable */
/* eslint-disable */
import { CheckinType } from './checkin-type';
import { SessionQuestionDetailsDto } from './session-question-details-dto';
export interface SessionCheckinDetailsDto {
  checkinType: CheckinType;
  questions?: null | Array<SessionQuestionDetailsDto>;
  sessionCheckinId: string;
  sessionId: string;
}
