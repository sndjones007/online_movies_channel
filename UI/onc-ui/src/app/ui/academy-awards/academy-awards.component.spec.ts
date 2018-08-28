import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AcademyAwardsComponent } from './academy-awards.component';

describe('AcademyAwardsComponent', () => {
  let component: AcademyAwardsComponent;
  let fixture: ComponentFixture<AcademyAwardsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AcademyAwardsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AcademyAwardsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
