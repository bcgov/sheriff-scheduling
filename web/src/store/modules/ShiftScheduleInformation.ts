import { shiftRangeInfoType, courtAdminAvailabilityInfoType, distributeTeamMemberInfoType } from '@/types/ShiftSchedule';
import { VuexModule, Module, Mutation, Action } from 'vuex-module-decorators'

@Module({
  namespaced: true
})
class ShiftScheduleInformation extends VuexModule {

  public shiftRangeInfo = {} as shiftRangeInfoType;
  public dailyShiftRangeInfo = {} as shiftRangeInfoType;
  public courtAdminsAvailabilityInfo = [] as courtAdminAvailabilityInfoType[];
  public selectedShifts = [] as string[];
  public teamMemberList = [] as distributeTeamMemberInfoType[];

  @Mutation
  public setShiftRangeInfo(shiftRangeInfo): void {   
    this.shiftRangeInfo = shiftRangeInfo
  }

  @Action
  public UpdateShiftRangeInfo(newShiftRangeInfo): void {
    this.context.commit('setShiftRangeInfo', newShiftRangeInfo)
  }

  @Mutation
  public setDailyShiftRangeInfo(dailyShiftRangeInfo): void {   
    this.dailyShiftRangeInfo = dailyShiftRangeInfo
  }

  @Action
  public UpdateDailyShiftRangeInfo(newDailyShiftRangeInfo): void {
    this.context.commit('setDailyShiftRangeInfo', newDailyShiftRangeInfo)
  }

  @Mutation
  public setCourtAdminsAvailabilityInfo(courtAdminsAvailabilityInfo): void {   
    this.courtAdminsAvailabilityInfo = courtAdminsAvailabilityInfo
  }

  @Action
  public UpdateCourtAdminsAvailabilityInfo(newCourtAdminsAvailabilityInfo): void {
    this.context.commit('setCourtAdminsAvailabilityInfo', newCourtAdminsAvailabilityInfo)
  }

  @Mutation
  public setSelectedShifts(selectedShifts): void {   
    this.selectedShifts = selectedShifts
  }

  @Action
  public UpdateSelectedShifts(newSelectedShifts): void {
    this.context.commit('setSelectedShifts', newSelectedShifts)
  }

  @Mutation
  public setTeamMemberList(teamMemberList): void {   
    this.teamMemberList = teamMemberList
  }

  @Action
  public UpdateTeamMemberList(newTeamMemberList): void {
    this.context.commit('setTeamMemberList', newTeamMemberList)
  }
}

export default ShiftScheduleInformation