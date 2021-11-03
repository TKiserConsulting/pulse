/* tslint:disable */
/* eslint-disable */
import { GrantAvailabilityData } from './grant-availability-data';
export interface GrantResult {
  allow?: boolean;
  grantsAvailability?: null | Array<GrantAvailabilityData>;
}
