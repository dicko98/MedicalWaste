import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  MatDatepickerModule,
  MatInputModule,
  MatNativeDateModule,
  MatSelectModule,
  MatPaginatorModule,
  MatSelectChange,
  MatTableDataSource,
  MatTableModule
} from '@angular/material';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

const MaterialComponents = [
  MatDatepickerModule,
  MatNativeDateModule,
  MatInputModule,
  MatSelectModule,
  MatPaginatorModule,
  MatTableModule
];

@NgModule({
  exports: [MaterialComponents],
  imports: [
    CommonModule,
    MaterialComponents,
    ReactiveFormsModule,
    FormsModule  
  ]
})
export class MaterialModule { }
