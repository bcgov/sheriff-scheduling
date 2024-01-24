import { assignmentCardInfoType, assignmentCardWeekInfoType, dutyRangeInfoType, myTeamShiftInfoType, selectedDutyCardInfoType} from '@/types/DutyRoster';
import { VuexModule, Module, Mutation, Action } from 'vuex-module-decorators'

@Module({
  namespaced: true
})
class DutyRosterInformation extends VuexModule {

  public dutyRangeInfo = {} as dutyRangeInfoType;
  public shiftAvailabilityInfo: myTeamShiftInfoType[] = [];
  public dutyToBeEdited = '';
  public view24h = false;
  public courtAdminFullview = false;
  public printCourtAdminFullview = false;
  public displayFuelGauge = false;

  public zoomLevel = 100;

  public dutyRosterAssignments: assignmentCardInfoType[] = [];
  public dutyRosterAssignmentsWeek: assignmentCardWeekInfoType[] = [];

  public selectedDuties: selectedDutyCardInfoType[] = [];
  
  @Mutation
  public setDutyRangeInfo(dutyRangeInfo): void {   
    this.dutyRangeInfo = dutyRangeInfo
  }

  @Action
  public UpdateDutyRangeInfo(newDutyRangeInfo): void {
    this.context.commit('setDutyRangeInfo', newDutyRangeInfo)
  }

  @Mutation
  public setShiftAvailabilityInfo(shiftAvailabilityInfo): void {   
    this.shiftAvailabilityInfo = shiftAvailabilityInfo;
  }

  @Action
  public UpdateShiftAvailabilityInfo(newShiftAvailabilityInfo): void {
    this.context.commit('setShiftAvailabilityInfo', newShiftAvailabilityInfo)
  }

  @Mutation
  public setDutyToBeEdited(dutyToBeEdited): void {   
    this.dutyToBeEdited = dutyToBeEdited;
  }

  @Action
  public UpdateDutyToBeEdited(newDutyToBeEdited): void {
    this.context.commit('setDutyToBeEdited', newDutyToBeEdited)
  }

  @Mutation
  public setView24h(view24h: boolean): void {   
    this.view24h = view24h;
  }

  @Action
  public UpdateView24h(newView24h: boolean): void {
    this.context.commit('setView24h', newView24h)
  }

  @Mutation
  public setCourtAdminFullview(courtAdminFullview: boolean): void {   
    this.courtAdminFullview = courtAdminFullview;
  }
  @Action
  public UpdateCourtAdminFullview(newCourtAdminFullview: boolean): void {
    this.context.commit('setCourtAdminFullview', newCourtAdminFullview)
  }

  @Mutation
  public setDisplayFuelGauge(displayFuelGauge: boolean): void {   
    this.displayFuelGauge = displayFuelGauge;
  }
  @Action
  public UpdateDisplayFuelGauge(newDisplayFuelGauge: boolean): void {
    this.context.commit('setDisplayFuelGauge', newDisplayFuelGauge)
  }

  @Mutation
  public setPrintCourtAdminFullview(printCourtAdminFullview: boolean): void {   
    this.printCourtAdminFullview = printCourtAdminFullview;
  }
  @Action
  public UpdatePrintCourtAdminFullview(newPrintCourtAdminFullview: boolean): void {
    this.context.commit('setPrintCourtAdminFullview', newPrintCourtAdminFullview)
  }

  @Mutation
  public setZoomLevel(zoomLevel: number): void {   
    this.zoomLevel = zoomLevel;
  }
  @Action
  public UpdateZoomLevel(newZoomLevel: number): void {
    this.context.commit('setZoomLevel', newZoomLevel)
  }

  
  @Mutation
  public setSelectedDuties(selectedDuties): void {   
    this.selectedDuties = selectedDuties
  }

  @Action
  public UpdateSelectedDuties(newSelectedDuties): void {
    this.context.commit('setSelectedDuties', newSelectedDuties)
  }


  @Mutation
  public setDutyRosterAssignments(dutyRosterAssignments: assignmentCardInfoType[]): void {   
    this.dutyRosterAssignments = dutyRosterAssignments
  }

  @Action
  public UpdateDutyRosterAssignments(newDutyRosterAssignments: assignmentCardInfoType[]): void {
    this.context.commit('setDutyRosterAssignments', newDutyRosterAssignments)
  }
  
  
  @Mutation
  public setDutyRosterAssignmentsWeek(dutyRosterAssignmentsWeek: assignmentCardWeekInfoType[]): void {   
    this.dutyRosterAssignmentsWeek = dutyRosterAssignmentsWeek
  }

  @Action
  public UpdateDutyRosterAssignmentsWeek(newDutyRosterAssignmentsWeek: assignmentCardWeekInfoType[]): void {
    this.context.commit('setDutyRosterAssignmentsWeek', newDutyRosterAssignmentsWeek)
  }
  
}

export default DutyRosterInformation