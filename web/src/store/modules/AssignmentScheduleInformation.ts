import { assignmentCardWeekInfoType } from '@/types/DutyRoster';
import { shiftRangeInfoType, courtAdminAvailabilityInfoType, distributeTeamMemberInfoType, selectShiftInfoType } from '@/types/ShiftSchedule';
import { VuexModule, Module, Mutation, Action } from 'vuex-module-decorators'

@Module({
  namespaced: true
})
class AssignmentScheduleInformation extends VuexModule {

  public assignmentRangeInfo = {} as shiftRangeInfoType;
  public courtAdminsAvailabilityInfo = [] as courtAdminAvailabilityInfoType[];
  public selectedShifts = [] as selectShiftInfoType[];
  public teamMemberList = [] as distributeTeamMemberInfoType[];

  public dutyShiftAssignmentsWeek: assignmentCardWeekInfoType[] = [];

  public editDutyModalID =""
  public manageDutiesModalID =""

  @Mutation
  public setAssignmentRangeInfo(assignmentRangeInfo): void {   
    this.assignmentRangeInfo = assignmentRangeInfo
  }
  @Action
  public UpdateAssignmentRangeInfo(newAssignmentRangeInfo): void {
    this.context.commit('setAssignmentRangeInfo', newAssignmentRangeInfo)
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

  @Mutation
  public setDutyShiftAssignmentsWeek(dutyShiftAssignmentsWeek: assignmentCardWeekInfoType[]): void {   
    this.dutyShiftAssignmentsWeek = dutyShiftAssignmentsWeek
  }
  @Action
  public UpdateDutyShiftAssignmentsWeek(newDutyShiftAssignmentsWeek: assignmentCardWeekInfoType[]): void {
    this.context.commit('setDutyShiftAssignmentsWeek', newDutyShiftAssignmentsWeek)
  }

  @Mutation
  public setEditDutyModalID(editDutyModalID): void {   
    this.editDutyModalID = editDutyModalID;
  }
  @Action
  public UpdateEditDutyModalID(newEditDutyModalID): void {
    this.context.commit('setEditDutyModalID', newEditDutyModalID)
  }

  @Mutation
  public setManageDutiesModalID(manageDutiesModalID): void {   
    this.manageDutiesModalID = manageDutiesModalID;
  }
  @Action
  public UpdateManageDutiesModalID(newManageDutiesModalID): void {
    this.context.commit('setManageDutiesModalID', newManageDutiesModalID)
  }

}

export default AssignmentScheduleInformation