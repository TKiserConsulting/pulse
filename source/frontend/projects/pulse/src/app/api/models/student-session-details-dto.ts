/* tslint:disable */
/* eslint-disable */
import { InstructorEmoticonListItemDto } from './instructor-emoticon-list-item-dto';
import { SessionCheckinDetailsDto } from './session-checkin-details-dto';
import { SessionQuestionDetailsDto } from './session-question-details-dto';
export interface StudentSessionDetailsDto {
  activeCheckin?: null | SessionCheckinDetailsDto;
  code: string;
  emoticons: Array<InstructorEmoticonListItemDto>;
  id: string;
  questions?: null | Array<SessionQuestionDetailsDto>;
}
