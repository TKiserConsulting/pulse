/* tslint:disable */
/* eslint-disable */
import { SessionCheckinDetailsDto } from './session-checkin-details-dto';
import { SessionQuestionDetailsDto } from './session-question-details-dto';
export interface CheckinData {
  checkin: SessionCheckinDetailsDto;
  questions: Array<SessionQuestionDetailsDto>;
}
