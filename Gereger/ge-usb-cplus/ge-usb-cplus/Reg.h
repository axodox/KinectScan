#pragma once
#include "gerincrajz.h"
namespace Reg{
//			idõ,	derivált, tartopontszam, idõsor, fvsor
#define sm 10
	const int delta=8;

double Regru(float t, int der, int n, double * xx,  double * yy) {
	double x4=0;
	double x3=0;
	double x2=0;
	double x=0;
	double x2y=0;
	double xy=0;
	double y=0;
	for (int i=0; i<n ; i++) {
		x+=xx[i];
		x2+=xx[i]*xx[i];
		x3+=xx[i]*xx[i]*xx[i];
		x4+=xx[i]*xx[i]*xx[i]*xx[i];
		x2y+=xx[i]*xx[i]*yy[i];
		xy+=xx[i]*yy[i];
		y+=yy[i];
		if (yy[i]==-100) return 0;
	}
	double deta=x4*(x2*n-x*x)-x3*(x3*n-x*x2)+x2*(x3*x-x2*x2);
	double a=((x2*n-x*x)*x2y-(x3*n-x*x2)*xy+(x3*x-x2*x2)*y)/deta;
	double b=(-(x3*n-x*x2)*x2y+(x4*n-x2*x2)*xy-(x4*x-x3*x2)*y)/deta;
	double c=((x3*x-x2*x2)*x2y-(x4*x-x3*x2)*xy+(x4*x2-x3*x3)*y)/deta;
	switch (der) { 
		case 0:
			return a*t*t+b*t+c;
			break;
		case 1:
			return 2*a*t+b;
			break;
		case 2:
			return 2*a;
			break;
	}
}



void d1(int poz) {
		double xx[2*delta+1];
		double yy[2*delta+1];
		for (int i=0; i<2*delta+1; i++) {
			xx[i]=0;
			yy[i]=0;
		}
		int kzd;
			if (poz-delta>0) 
				kzd=poz-delta;
			else 
				kzd=poz;
			for (int k=kzd; k<kzd+2*delta+1; k++) 
				xx[k-kzd]=global::points[k].ivh-global::points[kzd].ivh;
			int darab=0;
			for (int k=kzd; k<kzd+2*delta+1; k++) 
			{
				if (global::points[k].ivh!=-100){
					yy[k-kzd]=global::points[k].xr;
					darab++;
				}
			}
			double xhn;
			if (darab>2*delta-1)
				xhn=Regru(global::points[kzd+delta].ivh-global::points[kzd].ivh,1, darab, xx,  yy); 
			else
				xhn=-100;
			global::points[poz].dx=xhn; 
			darab=0;
			for (int k=kzd; k<kzd+2*delta+1; k++) 
				{
					if (global::points[k].ivh!=-100) {
						yy[k-kzd]=global::points[k].yr;
						darab++;
					}
				}
			if (darab>2*delta-1)
					xhn=Regru(global::points[kzd+delta].ivh-global::points[kzd].ivh,1, darab, xx,  yy); 
				else
					xhn=-100;
			global::points[poz].dy=xhn;
			darab=0;
			for (int k=kzd; k<kzd+2*delta+1; k++) 
			{
				if (global::points[k].ivh!=-100) {
					yy[k-kzd]=global::points[k].zr;
					darab++;
				}
			}
			if (darab>2*delta-1)
				xhn=Regru(global::points[kzd+delta].ivh-global::points[kzd].ivh,1, darab, xx,  yy); 
			else
				xhn=-100;
			global::points[poz].dz=xhn;
}

void d2(int poz) {
		double xx[2*delta+1];
		double yy[2*delta+1];
		for (int i=0; i<2*delta+1; i++) {
			xx[i]=0;
			yy[i]=0;
		}
		int kzd;
			if (poz-delta>0) 
				kzd=poz-delta;
			else 
				kzd=poz;
			for (int k=kzd; k<kzd+2*delta+1; k++) 
				xx[k-kzd]=global::points[k].ivh-global::points[kzd].ivh;
			int darab=0;
			for (int k=kzd; k<kzd+2*delta+1; k++) 
			{
				if (global::points[k].ivh!=-100){
					yy[k-kzd]=global::points[k].dx;
					darab++;
				}
			}
			double xhn;
			if (darab>2*delta-1)
				xhn=Regru(global::points[kzd+delta].ivh-global::points[kzd].ivh,1, darab, xx,  yy); 
			else
				xhn=-100;
			global::points[poz].ddx=xhn; 
			darab=0;
			for (int k=kzd; k<kzd+2*delta+1; k++) 
				{
					if (global::points[k].ivh!=-100) {
						yy[k-kzd]=global::points[k].dy;
						darab++;
					}
				}
			if (darab>2*delta-1)
					xhn=Regru(global::points[kzd+delta].ivh-global::points[kzd].ivh,1, darab, xx,  yy); 
				else
					xhn=-100;
			global::points[poz].ddy=xhn;
			darab=0;
			for (int k=kzd; k<kzd+2*delta+1; k++) 
			{
				if (global::points[k].ivh!=-100) {
					yy[k-kzd]=global::points[k].dz;
					darab++;
				}
			}
			if (darab>2*delta-1)
				xhn=Regru(global::points[kzd+delta].ivh-global::points[kzd].ivh,1, darab, xx,  yy); 
			else
				xhn=-100;
			global::points[poz].ddz=xhn;
			double xxx=global::points[poz].ddx;
			double yyy=global::points[poz].ddy;
			double zzz=global::points[poz].ddz;
			zzz=zzz;
}
void ktr(int poz) {
	double tabs=sqrt(pow(global::points[poz].dx,2)+pow(global::points[poz].dy,2)+pow(global::points[poz].dz,2));
	if (tabs>1e-12)
	{
		global::kt[poz].x=global::points[poz].dx/tabs;
		global::kt[poz].y=global::points[poz].dy/tabs;
		global::kt[poz].z=global::points[poz].dz/tabs;
	}
	double rabs=sqrt(pow(global::points[poz].ddx,2)+pow(global::points[poz].ddy,2)+pow(global::points[poz].ddz,2));
	if (rabs>1e-12)
	{
		global::kn[poz].x=global::points[poz].ddx/rabs;
		global::kn[poz].y=global::points[poz].ddy/rabs;
		global::kn[poz].z=global::points[poz].ddz/rabs;
	}
	global::kb[poz].x=global::kt[poz].y*global::kn[poz].z-global::kt[poz].z*global::kn[poz].y;
	global::kb[poz].y=-(global::kt[poz].x*global::kn[poz].z-global::kt[poz].z*global::kn[poz].x);
	global::kb[poz].z=global::kt[poz].x*global::kn[poz].y-global::kt[poz].y*global::kn[poz].x;

}

	
}