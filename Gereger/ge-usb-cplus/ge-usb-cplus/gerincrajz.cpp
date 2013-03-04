#include "stdafx.h"
#include <math.h>
#include "c1.h"
#include "t12.h"
#include "t11.h"
#include "t10.h"
#include "t9.h"
#include "t8.h"
#include "t7.h"
#include "t6.h"
#include "t5.h"
#include "t4.h"
#include "t3.h"
#include "t2.h"
#include "t1.h"
#include "l5.h"
#include "l4.h"
#include "l3.h"
#include "l2.h"
#include "l1.h"
#include "c7.h"
#include "c6.h"
#include "c5.h"
#include "c4.h"
#include "c3.h"
#include "c2.h"
#include "gerincrajz.h"

using System::String;

class tt {
public:
	double tr[3][3];
};
class xst {
public:
	double xs[3];
};





void tinit(tt * t){
		t->tr[0][0]=1;
		t->tr[1][0]=0;
		t->tr[2][0]=0;
		t->tr[0][1]=0;
		t->tr[1][1]=1;
		t->tr[2][1]=0;
		t->tr[0][2]=0;
		t->tr[1][2]=0;
		t->tr[2][2]=1;
}
void alfa(int kt, double a, tt * t){
	double sta[3][3];
	double stb[3][3];
	for (int i=0; i<3; i++)
		for (int j=0; j<3; j++)
		{
			sta[i][j]=t->tr[i][j];
		}
	switch (kt)
	{
	case 1:
		stb[0][0]=1;
		stb[0][1]=0;
		stb[0][2]=0;
		
		stb[1][0]=0;
		stb[1][1]=cos(a/180*3.14);
		stb[1][2]=sin(a/180*3.14);
		stb[2][0]=0;
		stb[2][1]=-sin(a/180*3.14);
		stb[2][2]=cos(a/180*3.14);
	break;
	case 2:
		stb[0][0]=cos(a/180*3.14);
		stb[1][0]=0;
		stb[2][0]=sin(a/180*3.14);
		stb[0][1]=0;
		stb[1][1]=1;
		stb[2][1]=0;
		stb[0][2]=-sin(a/180*3.14);
		stb[1][2]=0;
		stb[2][2]=cos(a/180*3.14);
	break;
	case 3:
		stb[0][0]=cos(a/180*3.14);
		stb[1][0]=-sin(a/180*3.14);
		stb[2][0]=0;
		stb[0][1]=sin(a/180*3.14);
		stb[1][1]=cos(a/180*3.14);
		stb[2][1]=0;
		stb[0][2]=0;
		stb[1][2]=0;
		stb[2][2]=1;
	break;
	}
	for (int i=0; i<3; i++)
		for (int j=0; j<3; j++)
		{
			t->tr[i][j]=0;
			for (int k=0; k<3; k++)
				t->tr[i][j]+=stb[i][k]*sta[k][j];
		}
		int zt;
}

void bemozdit(int i,tt * t, float * xsn,float * xsc,float * C_normals,float * C_coords, int * C_indices){
					for (int jk=0; jk<3; jk++)
						xsn[jk]=C_normals[C_indices[i]*3+jk];
					for (int ik=0; ik<3; ik++)
					{
						C_normals[C_indices[i]*3+ik]=0;
						for (int jk=0; jk<3; jk++)
							C_normals[C_indices[i]*3+ik]+=t->tr[ik][jk]*xsn[jk];
					}
					for (int jk=0; jk<3; jk++)
						xsc[jk]=C_coords[C_indices[i]*3+jk];
					for (int ik=0; ik<3; ik++)
					{
						C_coords[C_indices[i]*3+ik]=0;
						for (int jk=0; jk<3; jk++)
							C_coords[C_indices[i]*3+ik]+=t->tr[ik][jk]*xsc[jk];
					}

}
void visszaallit(int i,float * xsn, float * xsc, float * C_normals,float * C_coords, int * C_indices){
					for (int jk=0; jk<3; jk++)
					{
						C_normals[C_indices[i]*3+jk]=xsn[jk];
						C_coords[C_indices[i]*3+jk]=xsc[jk];
					}
}
void szogek(double * c_, tt * t){
				tinit(t);
				//alfa(1,C1_a_1,t);
				alfa(1,c_[0],t);
				//alfa(2,C1_a_2,t);
				alfa(2,c_[1],t);
				//alfa(3,C1_a_3,t);
				alfa(3,c_[2],t);
				//double szog=asin(C1_T_y/C1_T_z)*180/3.1415;
				double szog=asin(c_[4]/c_[5])*180/3.1415;
				alfa(1,szog,t);
				//szog=-asin(C1_T_x/C1_T_z)*180/3.1415;
				szog=-asin(c_[3]/c_[5])*180/3.1415;
				alfa(2,szog,t);
				if (c_[6]>0)
				{
					//szog=-asin(C1_N_y/C1_N_x)*180/3.1415;
					szog=-asin(c_[7]/c_[6])*180/3.1415;
				}
				else
				{
					//szog=asin(C1_N_y/C1_N_x)*180/3.1415;
					szog=asin(c_[7]/c_[6])*180/3.1415;
				}
				alfa(3,szog,t);
}





void gerincki()
{
			if (!global::hatgerinc)
				return;
			double szog=0;
			tt * t=new (tt);
			float xsn[3];
			float xsc[3];
			glColor3d((float)182/256,(float)182/256,(float)137/256);
			if (!global::danka)
			{
				int c1_hszog=sizeof(C1_indices);
				c1_hszog/=12;
				//C1
				glBegin(GL_TRIANGLES);
				szogek(C1_,t);
				for (int i=0; i<3*c1_hszog; i++)//
					{
						glNormal3f(C1_normals[C1_indices[i]*3],
							       C1_normals[C1_indices[i]*3+1],        
								   C1_normals[C1_indices[i]*3+2]);					
						glVertex3f(10*(C1_x+global::gsc*C1_coords[C1_indices[i]*3]),
								   10*(C1_y+global::gsc*C1_coords[C1_indices[i]*3+1]),
								   10*(C1_z+global::gsc*C1_coords[C1_indices[i]*3+2]));
					}
				glEnd();
			}
			//T12
			int t12_hszog=sizeof(T12_indices);
			t12_hszog/=12;
			glBegin(GL_TRIANGLES);
			szogek(T12_,t);
			for (int i=0; i<3*t12_hszog; i++)//
				{
					glNormal3f(T12_normals[T12_indices[i]*3],
						       T12_normals[T12_indices[i]*3+1],        
							   T12_normals[T12_indices[i]*3+2]);					
					glVertex3d(10*(T12_x+global::gsc*T12_coords[T12_indices[i]*3]),
							   10*(T12_y+global::gsc*T12_coords[T12_indices[i]*3+1]),
							   10*(T12_z+global::gsc*T12_coords[T12_indices[i]*3+2]));
				}
			glEnd();
			//T11
			int t11_hszog=sizeof(T11_indices);
			t11_hszog/=12;
			glBegin(GL_TRIANGLES);
			szogek(T11_,t);
			for (int i=0; i<3*t11_hszog; i++)//
				{
					glNormal3f(T11_normals[T11_indices[i]*3],
						       T11_normals[T11_indices[i]*3+1],        
							   T11_normals[T11_indices[i]*3+2]);					
					glVertex3d(10*(T11_x+global::gsc*T11_coords[T11_indices[i]*3]),
							   10*(T11_y+global::gsc*T11_coords[T11_indices[i]*3+1]),
							   10*(T11_z+global::gsc*T11_coords[T11_indices[i]*3+2]));
				}
			glEnd();
			//T10
			int t10_hszog=sizeof(T10_indices);
			t10_hszog/=12;
			glBegin(GL_TRIANGLES);
			szogek(T10_,t);
			for (int i=0; i<3*t10_hszog; i++)//
				{
					glNormal3f(T10_normals[T10_indices[i]*3],
						       T10_normals[T10_indices[i]*3+1],        
							   T10_normals[T10_indices[i]*3+2]);					
					glVertex3d(10*(T10_x+global::gsc*T10_coords[T10_indices[i]*3]),
							   10*(T10_y+global::gsc*T10_coords[T10_indices[i]*3+1]),
							   10*(T10_z+global::gsc*T10_coords[T10_indices[i]*3+2]));
				}
			glEnd();
			//T9
			int t9_hszog=sizeof(T9_indices);
			t9_hszog/=12;
			glBegin(GL_TRIANGLES);
			szogek(T9_,t);
			for (int i=0; i<3*t9_hszog; i++)//
				{
					glNormal3f(T9_normals[T9_indices[i]*3],
						       T9_normals[T9_indices[i]*3+1],        
							   T9_normals[T9_indices[i]*3+2]);					
					glVertex3d(10*(T9_x+global::gsc*T9_coords[T9_indices[i]*3]),
							   10*(T9_y+global::gsc*T9_coords[T9_indices[i]*3+1]),
							   10*(T9_z+global::gsc*T9_coords[T9_indices[i]*3+2]));
				}
			glEnd();
			//T8
			int t8_hszog=sizeof(T8_indices);
			t8_hszog/=12;
			glBegin(GL_TRIANGLES);
			szogek(T8_,t);
			for (int i=0; i<3*t8_hszog; i++)//
				{
					glNormal3f(T8_normals[T8_indices[i]*3],
						       T8_normals[T8_indices[i]*3+1],        
							   T8_normals[T8_indices[i]*3+2]);					
					glVertex3d(10*(T8_x+global::gsc*T8_coords[T8_indices[i]*3]),
							   10*(T8_y+global::gsc*T8_coords[T8_indices[i]*3+1]),
							   10*(T8_z+global::gsc*T8_coords[T8_indices[i]*3+2]));
				}
			glEnd();
			//T7
			int t7_hszog=sizeof(T7_indices);
			t7_hszog/=12;
			glBegin(GL_TRIANGLES);
			szogek(T7_,t);
			for (int i=0; i<3*t7_hszog; i++)//
				{
					glNormal3f(T7_normals[T7_indices[i]*3],
						       T7_normals[T7_indices[i]*3+1],        
							   T7_normals[T7_indices[i]*3+2]);					
					glVertex3d(10*(T7_x+global::gsc*T7_coords[T7_indices[i]*3]),
							   10*(T7_y+global::gsc*T7_coords[T7_indices[i]*3+1]),
							   10*(T7_z+global::gsc*T7_coords[T7_indices[i]*3+2]));
				}
			glEnd();
			//T6
			int t6_hszog=sizeof(T6_indices);
			t6_hszog/=12;
			glBegin(GL_TRIANGLES);
			szogek(T6_,t);
			for (int i=0; i<3*t6_hszog; i++)//
				{
					glNormal3f(T6_normals[T6_indices[i]*3],
						       T6_normals[T6_indices[i]*3+1],        
							   T6_normals[T6_indices[i]*3+2]);					
					glVertex3d(10*(T6_x+global::gsc*T6_coords[T6_indices[i]*3]),
							   10*(T6_y+global::gsc*T6_coords[T6_indices[i]*3+1]),
							   10*(T6_z+global::gsc*T6_coords[T6_indices[i]*3+2]));
				}
			glEnd();
			//T5
			int t5_hszog=sizeof(T5_indices);
			t5_hszog/=12;
			glBegin(GL_TRIANGLES);
			szogek(T5_,t);
			for (int i=0; i<3*t5_hszog; i++)//
				{
					glNormal3f(T5_normals[T5_indices[i]*3],
						       T5_normals[T5_indices[i]*3+1],        
							   T5_normals[T5_indices[i]*3+2]);					
					glVertex3d(10*(T5_x+global::gsc*T5_coords[T5_indices[i]*3]),
							   10*(T5_y+global::gsc*T5_coords[T5_indices[i]*3+1]),
							   10*(T5_z+global::gsc*T5_coords[T5_indices[i]*3+2]));
				}
			glEnd();
			//T4
			int t4_hszog=sizeof(T4_indices);
			t4_hszog/=12;
			glBegin(GL_TRIANGLES);
			szogek(T4_,t);
			for (int i=0; i<3*t4_hszog; i++)//
				{
					glNormal3f(T4_normals[T4_indices[i]*3],
						       T4_normals[T4_indices[i]*3+1],        
							   T4_normals[T4_indices[i]*3+2]);					
					glVertex3d(10*(T4_x+global::gsc*T4_coords[T4_indices[i]*3]),
							   10*(T4_y+global::gsc*T4_coords[T4_indices[i]*3+1]),
							   10*(T4_z+global::gsc*T4_coords[T4_indices[i]*3+2]));
				}
			glEnd();
			//T3
			int t3_hszog=sizeof(T3_indices);
			t3_hszog/=12;
			glBegin(GL_TRIANGLES);
			szogek(T3_,t);
			for (int i=0; i<3*t3_hszog; i++)//
				{
					glNormal3f(T3_normals[T3_indices[i]*3],
						       T3_normals[T3_indices[i]*3+1],        
							   T3_normals[T3_indices[i]*3+2]);					
					glVertex3d(10*(T3_x+global::gsc*T3_coords[T3_indices[i]*3]),
							   10*(T3_y+global::gsc*T3_coords[T3_indices[i]*3+1]),
							   10*(T3_z+global::gsc*T3_coords[T3_indices[i]*3+2]));
				}
			glEnd();
			//T2
			int t2_hszog=sizeof(T2_indices);
			t2_hszog/=12;
			glBegin(GL_TRIANGLES);
			szogek(T2_,t);
			for (int i=0; i<3*t2_hszog; i++)//
				{
					glNormal3f(T2_normals[T2_indices[i]*3],
						       T2_normals[T2_indices[i]*3+1],        
							   T2_normals[T2_indices[i]*3+2]);					
					glVertex3d(10*(T2_x+global::gsc*T2_coords[T2_indices[i]*3]),
							   10*(T2_y+global::gsc*T2_coords[T2_indices[i]*3+1]),
							   10*(T2_z+global::gsc*T2_coords[T2_indices[i]*3+2]));
				}
			glEnd();
			//T1
			int t1_hszog=sizeof(T1_indices);
			t1_hszog/=12;
			glBegin(GL_TRIANGLES);
			szogek(T1_,t);
			for (int i=0; i<3*t1_hszog; i++)//
				{
					glNormal3f(T1_normals[T1_indices[i]*3],
						       T1_normals[T1_indices[i]*3+1],        
							   T1_normals[T1_indices[i]*3+2]);					
					glVertex3d(10*(T1_x+global::gsc*T1_coords[T1_indices[i]*3]),
							   10*(T1_y+global::gsc*T1_coords[T1_indices[i]*3+1]),
							   10*(T1_z+global::gsc*T1_coords[T1_indices[i]*3+2]));
				}
			glEnd();
			//L5
			int l5_hszog=sizeof(L5_indices);
			l5_hszog/=12;
			glBegin(GL_TRIANGLES);
			szogek(L5_,t);
			for (int i=0; i<3*l5_hszog; i++)//
				{
					glNormal3f(L5_normals[L5_indices[i]*3],
						       L5_normals[L5_indices[i]*3+1],        
							   L5_normals[L5_indices[i]*3+2]);					
					glVertex3d(10*(L5_x+global::gsc*L5_coords[L5_indices[i]*3]),
							   10*(L5_y+global::gsc*L5_coords[L5_indices[i]*3+1]),
							   10*(L5_z+global::gsc*L5_coords[L5_indices[i]*3+2]));
				}
			glEnd();
			//L4
			int l4_hszog=sizeof(L4_indices);
			l4_hszog/=12;
			glBegin(GL_TRIANGLES);
			szogek(L4_,t);
			for (int i=0; i<3*l4_hszog; i++)//
				{
					glNormal3f(L4_normals[L4_indices[i]*3],
						       L4_normals[L4_indices[i]*3+1],        
							   L4_normals[L4_indices[i]*3+2]);					
					glVertex3d(10*(L4_x+global::gsc*L4_coords[L4_indices[i]*3]),
							   10*(L4_y+global::gsc*L4_coords[L4_indices[i]*3+1]),
							   10*(L4_z+global::gsc*L4_coords[L4_indices[i]*3+2]));
				}
			glEnd();
			//L3
			int l3_hszog=sizeof(L3_indices);
			l3_hszog/=12;
			glBegin(GL_TRIANGLES);
			szogek(L3_,t);
			for (int i=0; i<3*l3_hszog; i++)//
				{
					glNormal3f(L3_normals[L3_indices[i]*3],
						       L3_normals[L3_indices[i]*3+1],        
							   L3_normals[L3_indices[i]*3+2]);					
					glVertex3d(10*(L3_x+global::gsc*L3_coords[L3_indices[i]*3]),
							   10*(L3_y+global::gsc*L3_coords[L3_indices[i]*3+1]),
							   10*(L3_z+global::gsc*L3_coords[L3_indices[i]*3+2]));
				}
			glEnd();
			//L2
			int l2_hszog=sizeof(L2_indices);
			l2_hszog/=12;
			glBegin(GL_TRIANGLES);
			szogek(L2_,t);
			for (int i=0; i<3*l2_hszog; i++)//
				{
					glNormal3f(L2_normals[L2_indices[i]*3],
						       L2_normals[L2_indices[i]*3+1],        
							   L2_normals[L2_indices[i]*3+2]);					
					glVertex3d(10*(L2_x+global::gsc*L2_coords[L2_indices[i]*3]),
							   10*(L2_y+global::gsc*L2_coords[L2_indices[i]*3+1]),
							   10*(L2_z+global::gsc*L2_coords[L2_indices[i]*3+2]));
				}
			glEnd();
			//L1
			int l1_hszog=sizeof(L1_indices);
			l1_hszog/=12;
			glBegin(GL_TRIANGLES);
			szogek(L1_,t);
			for (int i=0; i<3*l1_hszog; i++)//
				{
					glNormal3f(L1_normals[L1_indices[i]*3],
						       L1_normals[L1_indices[i]*3+1],        
							   L1_normals[L1_indices[i]*3+2]);					
					glVertex3d(10*(L1_x+global::gsc*L1_coords[L1_indices[i]*3]),
							   10*(L1_y+global::gsc*L1_coords[L1_indices[i]*3+1]),
							   10*(L1_z+global::gsc*L1_coords[L1_indices[i]*3+2]));
				}
			glEnd();
  			if (!global::danka)
			{
				//C7
				int c7_hszog=sizeof(C7_indices);
				c7_hszog/=12;
				glBegin(GL_TRIANGLES);
					szogek(C7_,t);
					for (int i=0; i<3*c7_hszog; i++)//
					{
						glNormal3f(C7_normals[C7_indices[i]*3],
							       C7_normals[C7_indices[i]*3+1],        
								   C7_normals[C7_indices[i]*3+2]);					
						glVertex3d(10*(C7_x+C7_coords[C7_indices[i]*3]),
								   10*(C7_y+C7_coords[C7_indices[i]*3+1]),
								   10*(C7_z+C7_coords[C7_indices[i]*3+2]));
					}
				glEnd();
				//C6
				int c6_hszog=sizeof(C6_indices);
				c6_hszog/=12;
				glBegin(GL_TRIANGLES);
					szogek(C6_,t);
					for (int i=0; i<3*c6_hszog; i++)//
					{
						glNormal3f(C6_normals[C6_indices[i]*3],
							       C6_normals[C6_indices[i]*3+1],        
								   C6_normals[C6_indices[i]*3+2]);					
						glVertex3d(10*(C6_x+C6_coords[C6_indices[i]*3]),
									10*(C6_y+C6_coords[C6_indices[i]*3+1]),
									10*(C6_z+C6_coords[C6_indices[i]*3+2]));
					}
				glEnd();
				//C5
				int c5_hszog=sizeof(C5_indices);
				c5_hszog/=12;
				glBegin(GL_TRIANGLES);
					szogek(C5_,t);
					for (int i=0; i<3*c5_hszog; i++)//
					{
						glNormal3f(C5_normals[C5_indices[i]*3],
							       C5_normals[C5_indices[i]*3+1],        
								   C5_normals[C5_indices[i]*3+2]);					
						glVertex3d(10*(C5_x+C5_coords[C5_indices[i]*3]),
								   10*(C5_y+C5_coords[C5_indices[i]*3+1]),
								   10*(C5_z+C5_coords[C5_indices[i]*3+2]));
					}
				glEnd();
				//C4  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
				int c4_hszog=sizeof(C3_indices);
				c4_hszog/=12;
				glBegin(GL_TRIANGLES);
					szogek(C4_,t);
					for (int i=0; i<3*c4_hszog; i++)//
					{
						int ic=C3_indices[i];
						glNormal3f(C3_normals[C3_indices[i]*3],
							       C3_normals[C3_indices[i]*3+1],        
								   C3_normals[C3_indices[i]*3+2]);					
						glVertex3d(10*(C4_x+C3_coords[C3_indices[i]*3]),
								   10*(C4_y+C3_coords[C3_indices[i]*3+1]),
								   10*(C4_z+C3_coords[C3_indices[i]*3+2]));
					}
				glEnd();
				//C3
				int c3_hszog=sizeof(C3_indices);
				c3_hszog/=12;
 				glBegin(GL_TRIANGLES);
					szogek(C3_,t);
					for (int i=0; i<3*c3_hszog; i++)//
					{
						glNormal3f(C3_normals[C3_indices[i]*3],
							       C3_normals[C3_indices[i]*3+1],        
								   C3_normals[C3_indices[i]*3+2]);					
						glVertex3d(10*(C3_x+C3_coords[C3_indices[i]*3]),
								   10*(C3_y+C3_coords[C3_indices[i]*3+1]),
									10*(C3_z+C3_coords[C3_indices[i]*3+2]));
					}
				glEnd();

				//C2
				int c2_hszog=sizeof(C2_indices);
				c2_hszog/=12;
				glBegin(GL_TRIANGLES);
					szogek(C2_,t);
					for (int i=0; i<3*c2_hszog; i++)//
					{
						glNormal3f(C2_normals[C2_indices[i]*3],
							       C2_normals[C2_indices[i]*3+1],        
								   C2_normals[C2_indices[i]*3+2]);					
						glVertex3d(10*(C2_x+C2_coords[C2_indices[i]*3]),
								   10*(C2_y+C2_coords[C2_indices[i]*3+1]),
								   10*(C2_z+C2_coords[C2_indices[i]*3+2]));
					}
				glEnd();
			}

}
double alap_ivhossz()
{
	double ivh=0;
	if (!global::danka)
	{
		global::csig_iv[0]=0;
		ivh+=sqrt(pow(C2_x-C1_x,2)+pow(C2_y-C1_y,2)+pow(C2_z-C1_z,2));
		global::csig_iv[1]=10*ivh;
		ivh+=sqrt(pow(C3_x-C2_x,2)+pow(C3_y-C2_y,2)+pow(C3_z-C2_z,2));
		global::csig_iv[2]=10*ivh;
		ivh+=sqrt(pow(C4_x-C3_x,2)+pow(C4_y-C3_y,2)+pow(C4_z-C3_z,2));
		global::csig_iv[3]=10*ivh;
		ivh+=sqrt(pow(C5_x-C4_x,2)+pow(C5_y-C4_y,2)+pow(C5_z-C4_z,2));
		global::csig_iv[4]=10*ivh;
		ivh+=sqrt(pow(C6_x-C5_x,2)+pow(C6_y-C5_y,2)+pow(C6_z-C5_z,2));
		global::csig_iv[5]=10*ivh;
		ivh+=sqrt(pow(C7_x-C6_x,2)+pow(C7_y-C6_y,2)+pow(C7_z-C6_z,2));
		global::csig_iv[6]=10*ivh;
	
		ivh+=sqrt(pow(T1_x-C7_x,2)+pow(T1_y-C7_y,2)+pow(T1_z-C7_z,2));
		global::csig_iv[7]=10*ivh;
		
		ivh+=sqrt(pow(T2_x-T1_x,2)+pow(T2_y-T1_y,2)+pow(T2_z-T1_z,2));
		global::csig_iv[8]=10*ivh;
		ivh+=sqrt(pow(T3_x-T2_x,2)+pow(T3_y-T2_y,2)+pow(T3_z-T2_z,2));
		global::csig_iv[9]=10*ivh;
		ivh+=sqrt(pow(T4_x-T3_x,2)+pow(T4_y-T3_y,2)+pow(T4_z-T3_z,2));
		global::csig_iv[10]=10*ivh;
		ivh+=sqrt(pow(T5_x-T4_x,2)+pow(T5_y-T4_y,2)+pow(T5_z-T4_z,2));
		global::csig_iv[11]=10*ivh;
		ivh+=sqrt(pow(T6_x-T5_x,2)+pow(T6_y-T5_y,2)+pow(T6_z-T5_z,2));
		global::csig_iv[12]=10*ivh;
		ivh+=sqrt(pow(T7_x-T6_x,2)+pow(T7_y-T6_y,2)+pow(T7_z-T6_z,2));
		global::csig_iv[13]=10*ivh;
		ivh+=sqrt(pow(T8_x-T7_x,2)+pow(T8_y-T7_y,2)+pow(T8_z-T7_z,2));
		global::csig_iv[14]=10*ivh;
		ivh+=sqrt(pow(T9_x-T8_x,2)+pow(T9_y-T8_y,2)+pow(T9_z-T8_z,2));
		global::csig_iv[15]=10*ivh;
		ivh+=sqrt(pow(T10_x-T9_x,2)+pow(T10_y-T9_y,2)+pow(T10_z-T9_z,2));
		global::csig_iv[16]=10*ivh;
		ivh+=sqrt(pow(T11_x-T10_x,2)+pow(T11_y-T10_y,2)+pow(T11_z-T10_z,2));
		global::csig_iv[17]=10*ivh;
		ivh+=sqrt(pow(T12_x-T11_x,2)+pow(T12_y-T11_y,2)+pow(T12_z-T11_z,2));
		global::csig_iv[18]=10*ivh;
	
		ivh+=sqrt(pow(L1_x-T12_x,2)+pow(L1_y-T12_y,2)+pow(L1_z-T12_z,2));
		global::csig_iv[19]=10*ivh;
	
		ivh+=sqrt(pow(L2_x-L1_x,2)+pow(L2_y-L1_y,2)+pow(L2_z-L1_z,2));
		global::csig_iv[20]=10*ivh;
		ivh+=sqrt(pow(L3_x-L2_x,2)+pow(L3_y-L2_y,2)+pow(L3_z-L2_z,2));
		global::csig_iv[21]=10*ivh;
		ivh+=sqrt(pow(L4_x-L3_x,2)+pow(L4_y-L3_y,2)+pow(L4_z-L3_z,2));
		global::csig_iv[22]=10*ivh;
		ivh+=sqrt(pow(L5_x-L4_x,2)+pow(L5_y-L4_y,2)+pow(L5_z-L4_z,2));
		global::csig_iv[23]=10*ivh;
	}
	else
	{
		ivh+=sqrt(pow(T2_x-T1_x,2)+pow(T2_y-T1_y,2)+pow(T2_z-T1_z,2));
		global::csig_iv[8]=10*ivh;
		ivh+=sqrt(pow(T3_x-T2_x,2)+pow(T3_y-T2_y,2)+pow(T3_z-T2_z,2));
		global::csig_iv[9]=10*ivh;
		ivh+=sqrt(pow(T4_x-T3_x,2)+pow(T4_y-T3_y,2)+pow(T4_z-T3_z,2));
		global::csig_iv[10]=10*ivh;
		ivh+=sqrt(pow(T5_x-T4_x,2)+pow(T5_y-T4_y,2)+pow(T5_z-T4_z,2));
		global::csig_iv[11]=10*ivh;
		ivh+=sqrt(pow(T6_x-T5_x,2)+pow(T6_y-T5_y,2)+pow(T6_z-T5_z,2));
		global::csig_iv[12]=10*ivh;
		ivh+=sqrt(pow(T7_x-T6_x,2)+pow(T7_y-T6_y,2)+pow(T7_z-T6_z,2));
		global::csig_iv[13]=10*ivh;
		ivh+=sqrt(pow(T8_x-T7_x,2)+pow(T8_y-T7_y,2)+pow(T8_z-T7_z,2));
		global::csig_iv[14]=10*ivh;
		ivh+=sqrt(pow(T9_x-T8_x,2)+pow(T9_y-T8_y,2)+pow(T9_z-T8_z,2));
		global::csig_iv[15]=10*ivh;
		ivh+=sqrt(pow(T10_x-T9_x,2)+pow(T10_y-T9_y,2)+pow(T10_z-T9_z,2));
		global::csig_iv[16]=10*ivh;
		ivh+=sqrt(pow(T11_x-T10_x,2)+pow(T11_y-T10_y,2)+pow(T11_z-T10_z,2));
		global::csig_iv[17]=10*ivh;
		ivh+=sqrt(pow(T12_x-T11_x,2)+pow(T12_y-T11_y,2)+pow(T12_z-T11_z,2));
		global::csig_iv[18]=10*ivh;
	
		ivh+=sqrt(pow(L1_x-T12_x,2)+pow(L1_y-T12_y,2)+pow(L1_z-T12_z,2));
		global::csig_iv[19]=10*ivh;
	
		ivh+=sqrt(pow(L2_x-L1_x,2)+pow(L2_y-L1_y,2)+pow(L2_z-L1_z,2));
		global::csig_iv[20]=10*ivh;
		ivh+=sqrt(pow(L3_x-L2_x,2)+pow(L3_y-L2_y,2)+pow(L3_z-L2_z,2));
		global::csig_iv[21]=10*ivh;
		ivh+=sqrt(pow(L4_x-L3_x,2)+pow(L4_y-L3_y,2)+pow(L4_z-L3_z,2));
		global::csig_iv[22]=10*ivh;
		ivh+=sqrt(pow(L5_x-L4_x,2)+pow(L5_y-L4_y,2)+pow(L5_z-L4_z,2));
		global::csig_iv[23]=10*ivh;
	}
	return 10*ivh;
}

bool pontosit(int cs, int p, double * x, double * y, double * z,  
							 double * dx, double * dy, double * dz,  
							 double * ddx, double * ddy, double * ddz, 
							 double * torz)
{
	double oiv=0;
	for (int i=0; i<p-1; i++)
	{
		for (int j=0; j<101; j++)
		{
			oiv+=sqrt(pow((global::points[i+1].xr-global::points[i].xr)/100,2)+
					 pow((global::points[i+1].yr-global::points[i].yr)/100,2)+ 	
					 pow((global::points[i+1].zr-global::points[i].zr)/100,2));
		}
	}
	double iv=0;
	for (int i=0; i<p-1; i++)
	{
		for (int j=0; j<101; j++)
		{
			double div=sqrt(pow((global::points[i+1].xr-global::points[i].xr)/100,2)+
					 pow((global::points[i+1].yr-global::points[i].yr)/100,2)+ 	
					 pow((global::points[i+1].zr-global::points[i].zr)/100,2));
			iv+=div;
			if (iv/oiv>=global::csig_iv[cs]/global::csig_iv[23])
			{
				*x=global::points[i].xr+(global::points[i+1].xr-global::points[i].xr)*j/100;
				*y=global::points[i].yr+(global::points[i+1].yr-global::points[i].yr)*j/100;
				*z=global::points[i].zr+(global::points[i+1].zr-global::points[i].zr)*j/100;
				if (div>0) 
				{
					*dx=(global::points[i+1].xr-global::points[i].xr)/div;
					*dy=(global::points[i+1].yr-global::points[i].yr)/div;
					*dz=(global::points[i+1].zr-global::points[i].zr)/div;
					*ddx=((global::points[i+1].xr-global::points[i].xr)-(global::points[i].xr-global::points[i-1].xr))/div;
					*ddy=((global::points[i+1].yr-global::points[i].yr)-(global::points[i].yr-global::points[i-1].yr))/div;
					*ddz=((global::points[i+1].zr-global::points[i].zr)-(global::points[i].zr-global::points[i-1].zr))/div;
					 double eddx=((global::points[i+2].xr-global::points[i+1].xr)-(global::points[i+1].xr-global::points[i].xr))/div;
					 double eddy=((global::points[i+2].yr-global::points[i+1].yr)-(global::points[i+1].yr-global::points[i].yr))/div;
					 double eddz=((global::points[i+2].zr-global::points[i+1].zr)-(global::points[i+1].zr-global::points[i].zr))/div;

					double dddx=(eddx-*ddx)/div;
					double dddy=(eddy-*ddy)/div;
					double dddz=(eddz-*ddz)/div;
					// (r'Xr'')*r'''/(|r'Xr''|2)
					// r'=(dx,dy,dz)
					// r''=(ddx,ddy,ddz)
					double szam=dddx*(*dy**ddz-*dz**ddy)+dddy*(*dz**ddx-*dx**ddz)+dddz*(*dx**ddy-*dy**ddx);
					double nev2=pow(*dy**ddz-*dz**ddy,2)+pow(*dz**ddx-*dx**ddz,2)+pow(*dx**ddy-*dy**ddx,3); 
					*torz=szam/nev2;

				}
				else 
				{
					*dx=0;
					*dy=0;
					*dz=0;
					*ddx=0;
					*ddy=0;
					*ddz=0;
					*torz=0;
				}

				return true;
			}
		}
	}
	return false;
}



void passz(int mp, int pdb)
{
	int csig=1;
	double kezd_mert_iv=0;
	if (!global::danka)
	{
		csig=1;
		C1_x=0;
		C1_y=0;
		C1_z=0;
		C1_[3]=global::kt[1].x;
		C1_[4]=global::kt[1].y;
		C1_[5]=global::kt[1].z;
		C1_[6]=global::kn[1].x;
		C1_[7]=global::kn[1].y;
		C1_[8]=global::kn[1].z;
		C1_[9]=global::kb[1].x;
		C1_[10]=global::kb[1].y;
		C1_[11]=global::kb[1].z;
		kezd_mert_iv=0;
	}
	else
	{
		csig=8;
		T1_x=0;
		T1_y=0;
		T1_z=0;
		T1_[3]=global::kt[1].x;
		T1_[4]=global::kt[1].y;
		T1_[5]=global::kt[1].z;
		T1_[6]=global::kn[1].x;
		T1_[7]=global::kn[1].y;
		T1_[8]=global::kn[1].z;
		T1_[9]=global::kb[1].x;
		T1_[10]=global::kb[1].y;
		T1_[11]=global::kb[1].z;
		kezd_mert_iv=0;
	}


	for (int i=1; i<mp; i++)
	{
		double miv=global::mert_iv[i];
		double oh=global::iv_mert*global::csig_iv[csig]/global::iv_alap;
		global::gsc=global::iv_mert/global::iv_alap;
		double x,y,z, dx, dy, dz, ddx, ddy, ddz, gorb;
		if (global::mert_iv[i]>global::iv_mert*global::csig_iv[csig]/global::iv_alap)
		{
			switch (csig)
			{
			case 1:
				pontosit(1, pdb, &x,  &y, &z, &dx,  &dy, &dz, &ddx,  &ddy, &ddz, &gorb);
				//C2_x=0.1F*global::mert_x[i];
				//C2_y=0.1F*global::mert_y[i];
				//C2_z=0.1F*global::mert_z[i];
				C2_x=0.1F*x;
				C2_y=0.1F*y;
				C2_z=0.1F*z;

				C2_dx=0.1F*dx;
				C2_dy=0.1F*dy;
				C2_dz=0.1F*dz;
				C2_ddx=0.1F*ddx;
				C2_ddy=0.1F*ddy;
				C2_ddz=0.1F*ddz;
				C2_[3]=x;//global::kt[i].x;
				C2_[4]=global::kt[i].y;
				C2_[5]=global::kt[i].z;
				C2_[6]=global::kn[i].x;
				C2_[7]=global::kn[i].y;
				C2_[8]=global::kn[i].z;
				C2_[9]=global::kb[i].x;
				C2_[10]=global::kb[i].y;
				C2_[11]=global::kb[i].z;
				C2_[12]=gorb;
				break;
			case 2:
				pontosit(2, pdb, &x,  &y, &z, &dx,  &dy, &dz, &ddx,  &ddy, &ddz, &gorb);
				//C3_x=0.1F*global::mert_x[i];
				//C3_y=0.1F*global::mert_y[i];
				//C3_z=0.1F*global::mert_z[i];
				C3_x=0.1F*x;
				C3_y=0.1F*y;
				C3_z=0.1F*z;

				C3_dx=0.1F*dx;
				C3_dy=0.1F*dy;
				C3_dz=0.1F*dz;
				C3_ddx=0.1F*ddx;
				C3_ddy=0.1F*ddy;
				C3_ddz=0.1F*ddz;

				C3_[3]=global::kt[i].x;
				C3_[4]=global::kt[i].y;
				C3_[5]=global::kt[i].z;
				C3_[6]=global::kn[i].x;
				C3_[7]=global::kn[i].y;
				C3_[8]=global::kn[i].z;
				C3_[9]=global::kb[i].x;
				C3_[10]=global::kb[i].y;
				C3_[11]=global::kb[i].z;
				C3_[12]=gorb;

				break;
			case 3:
				pontosit(3, pdb, &x,  &y, &z, &dx,  &dy, &dz, &ddx,  &ddy, &ddz, &gorb);
				//C4_x=0.1F*global::mert_x[i];
				//C4_y=0.1F*global::mert_y[i];
				//C4_z=0.1F*global::mert_z[i];
				C4_x=0.1F*x;
				C4_y=0.1F*y;
				C4_z=0.1F*z;

				C4_dx=0.1F*dx;
				C4_dy=0.1F*dy;
				C4_dz=0.1F*dz;
				C4_ddx=0.1F*ddx;
				C4_ddy=0.1F*ddy;
				C4_ddz=0.1F*ddz;

				C4_[3]=global::kt[i].x;
				C4_[4]=global::kt[i].y;
				C4_[5]=global::kt[i].z;
				C4_[6]=global::kn[i].x;
				C4_[7]=global::kn[i].y;
				C4_[8]=global::kn[i].z;
				C4_[9]=global::kb[i].x;
				C4_[10]=global::kb[i].y;
				C4_[11]=global::kb[i].z;
				C4_[12]=gorb;

				break;
			case 4:
				pontosit(4, pdb, &x,  &y, &z, &dx,  &dy, &dz, &ddx,  &ddy, &ddz, &gorb);
				//C5_x=0.1F*global::mert_x[i];
				//C5_y=0.1F*global::mert_y[i];
				//C5_z=0.1F*global::mert_z[i];
				C5_x=0.1F*x;
				C5_y=0.1F*y;
				C5_z=0.1F*z;

				C5_dx=0.1F*dx;
				C5_dy=0.1F*dy;
				C5_dz=0.1F*dz;
				C5_ddx=0.1F*ddx;
				C5_ddy=0.1F*ddy;
				C5_ddz=0.1F*ddz;

				C5_[3]=global::kt[i].x;
				C5_[4]=global::kt[i].y;
				C5_[5]=global::kt[i].z;
				C5_[6]=global::kn[i].x;
				C5_[7]=global::kn[i].y;
				C5_[8]=global::kn[i].z;
				C5_[9]=global::kb[i].x;
				C5_[10]=global::kb[i].y;
				C5_[11]=global::kb[i].z;
				C5_[12]=gorb;
				break;
			case 5:
				pontosit(5, pdb, &x,  &y, &z, &dx,  &dy, &dz, &ddx,  &ddy, &ddz, &gorb);
				//C6_x=0.1F*global::mert_x[i];
				//C6_y=0.1F*global::mert_y[i];
				//C6_z=0.1F*global::mert_z[i];
				C6_x=0.1F*x;
				C6_y=0.1F*y;
				C6_z=0.1F*z;

				C6_dx=0.1F*dx;
				C6_dy=0.1F*dy;
				C6_dz=0.1F*dz;
				C6_ddx=0.1F*ddx;
				C6_ddy=0.1F*ddy;
				C6_ddz=0.1F*ddz;

				C6_[3]=global::kt[i].x;
				C6_[4]=global::kt[i].y;
				C6_[5]=global::kt[i].z;
				C6_[6]=global::kn[i].x;
				C6_[7]=global::kn[i].y;
				C6_[8]=global::kn[i].z;
				C6_[9]=global::kb[i].x;
				C6_[10]=global::kb[i].y;
				C6_[11]=global::kb[i].z;
				C6_[12]=gorb;
				break;
			case 6:
				pontosit(6, pdb, &x,  &y, &z, &dx,  &dy, &dz, &ddx,  &ddy, &ddz, &gorb);
				//C7_x=0.1F*global::mert_x[i];
				//C7_y=0.1F*global::mert_y[i];
				//C7_z=0.1F*global::mert_z[i];
				C7_x=0.1F*x;
				C7_y=0.1F*y;
				C7_z=0.1F*z;

				C7_dx=0.1F*dx;
				C7_dy=0.1F*dy;
				C7_dz=0.1F*dz;
				C7_ddx=0.1F*ddx;
				C7_ddy=0.1F*ddy;
				C7_ddz=0.1F*ddz;

				C7_[3]=global::kt[i].x;
				C7_[4]=global::kt[i].y;
				C7_[5]=global::kt[i].z;
				C7_[6]=global::kn[i].x;
				C7_[7]=global::kn[i].y;
				C7_[8]=global::kn[i].z;
				C7_[9]=global::kb[i].x;
				C7_[10]=global::kb[i].y;
				C7_[11]=global::kb[i].z;
				C7_[12]=gorb;

				break;
			case 7:
				pontosit(7, pdb, &x,  &y, &z, &dx,  &dy, &dz, &ddx,  &ddy, &ddz, &gorb);
				//T1_x=0.1F*global::mert_x[i];
				//T1_y=0.1F*global::mert_y[i];
				//T1_z=0.1F*global::mert_z[i];
				T1_x=0.1F*x;
				T1_y=0.1F*y;
				T1_z=0.1F*z;

				T1_dx=0.1F*dx;
				T1_dy=0.1F*dy;
				T1_dz=0.1F*dz;
				T1_ddx=0.1F*ddx;
				T1_ddy=0.1F*ddy;
				T1_ddz=0.1F*ddz;


				T1_[3]=global::kt[i].x;
				T1_[4]=global::kt[i].y;
				T1_[5]=global::kt[i].z;
				T1_[6]=global::kn[i].x;
				T1_[7]=global::kn[i].y;
				T1_[8]=global::kn[i].z;
				T1_[9]=global::kb[i].x;
				T1_[10]=global::kb[i].y;
				T1_[11]=global::kb[i].z;
				T1_[12]=gorb;
				break;
			case 8:
				pontosit(8, pdb, &x,  &y, &z, &dx,  &dy, &dz, &ddx,  &ddy, &ddz, &gorb);
				//T2_x=0.1F*global::mert_x[i];
				//T2_y=0.1F*global::mert_y[i];
				//T2_z=0.1F*global::mert_z[i];
				T2_x=0.1F*x;
				T2_y=0.1F*y;
				T2_z=0.1F*z;

				T2_dx=0.1F*dx;
				T2_dy=0.1F*dy;
				T2_dz=0.1F*dz;
				T2_ddx=0.1F*ddx;
				T2_ddy=0.1F*ddy;
				T2_ddz=0.1F*ddz;

				T2_[3]=global::kt[i].x;
				T2_[4]=global::kt[i].y;
				T2_[5]=global::kt[i].z;
				T2_[6]=global::kn[i].x;
				T2_[7]=global::kn[i].y;
				T2_[8]=global::kn[i].z;
				T2_[9]=global::kb[i].x;
				T2_[10]=global::kb[i].y;
				T2_[11]=global::kb[i].z;
				T2_[12]=gorb;

				break;
			case 9:
				pontosit(9, pdb, &x,  &y, &z, &dx,  &dy, &dz, &ddx,  &ddy, &ddz, &gorb);
				T3_x=0.1F*x;
				T3_y=0.1F*y;
				T3_z=0.1F*z;
				//T3_x=0.1F*global::mert_x[i];
				//T3_y=0.1F*global::mert_y[i];
				//T3_z=0.1F*global::mert_z[i];

				T3_dx=0.1F*dx;
				T3_dy=0.1F*dy;
				T3_dz=0.1F*dz;
				T3_ddx=0.1F*ddx;
				T3_ddy=0.1F*ddy;
				T3_ddz=0.1F*ddz;


				T3_[3]=global::kt[i].x;
				T3_[4]=global::kt[i].y;
				T3_[5]=global::kt[i].z;
				T3_[6]=global::kn[i].x;
				T3_[7]=global::kn[i].y;
				T3_[8]=global::kn[i].z;
				T3_[9]=global::kb[i].x;
				T3_[10]=global::kb[i].y;
				T3_[11]=global::kb[i].z;
				T3_[12]=gorb;
				break;
			case 10:
				pontosit(10, pdb, &x,  &y, &z, &dx,  &dy, &dz, &ddx,  &ddy, &ddz, &gorb);
				T4_x=0.1F*x;
				T4_y=0.1F*y;
				T4_z=0.1F*z;
				//T4_x=0.1F*global::mert_x[i];
				//T4_y=0.1F*global::mert_y[i];
				//T4_z=0.1F*global::mert_z[i];

				T4_dx=0.1F*dx;
				T4_dy=0.1F*dy;
				T4_dz=0.1F*dz;
				T4_ddx=0.1F*ddx;
				T4_ddy=0.1F*ddy;
				T4_ddz=0.1F*ddz;


				T4_[3]=global::kt[i].x;
				T4_[4]=global::kt[i].y;
				T4_[5]=global::kt[i].z;
				T4_[6]=global::kn[i].x;
				T4_[7]=global::kn[i].y;
				T4_[8]=global::kn[i].z;
				T4_[9]=global::kb[i].x;
				T4_[10]=global::kb[i].y;
				T4_[11]=global::kb[i].z;
				T4_[12]=gorb;

				break;
			case 11:
				pontosit(11, pdb, &x,  &y, &z, &dx,  &dy, &dz, &ddx,  &ddy, &ddz, &gorb);
				T5_x=0.1F*x;
				T5_y=0.1F*y;
				T5_z=0.1F*z;
				//T5_x=0.1F*global::mert_x[i];
				//T5_y=0.1F*global::mert_y[i];
				//T5_z=0.1F*global::mert_z[i];

				T5_dx=0.1F*dx;
				T5_dy=0.1F*dy;
				T5_dz=0.1F*dz;
				T5_ddx=0.1F*ddx;
				T5_ddy=0.1F*ddy;
				T5_ddz=0.1F*ddz;



				T5_[3]=global::kt[i].x;
				T5_[4]=global::kt[i].y;
				T5_[5]=global::kt[i].z;
				T5_[6]=global::kn[i].x;
				T5_[7]=global::kn[i].y;
				T5_[8]=global::kn[i].z;
				T5_[9]=global::kb[i].x;
				T5_[10]=global::kb[i].y;
				T5_[11]=global::kb[i].z;
				T5_[12]=gorb;

				break;
			case 12:
				pontosit(12, pdb, &x,  &y, &z, &dx,  &dy, &dz, &ddx,  &ddy, &ddz, &gorb);
				T6_x=0.1F*x;
				T6_y=0.1F*y;
				T6_z=0.1F*z;
				//T6_x=0.1F*global::mert_x[i];
				//T6_y=0.1F*global::mert_y[i];
				//T6_z=0.1F*global::mert_z[i];

				T6_dx=0.1F*dx;
				T6_dy=0.1F*dy;
				T6_dz=0.1F*dz;
				T6_ddx=0.1F*ddx;
				T6_ddy=0.1F*ddy;
				T6_ddz=0.1F*ddz;

				T6_[3]=global::kt[i].x;
				T6_[4]=global::kt[i].y;
				T6_[5]=global::kt[i].z;
				T6_[6]=global::kn[i].x;
				T6_[7]=global::kn[i].y;
				T6_[8]=global::kn[i].z;
				T6_[9]=global::kb[i].x;
				T6_[10]=global::kb[i].y;
				T6_[11]=global::kb[i].z;
				T6_[12]=gorb;

				break;
			case 13:
				pontosit(13, pdb, &x,  &y, &z, &dx,  &dy, &dz, &ddx,  &ddy, &ddz, &gorb);
				T7_x=0.1F*x;
				T7_y=0.1F*y;
				T7_z=0.1F*z;
				//T7_x=0.1F*global::mert_x[i];
				//T7_y=0.1F*global::mert_y[i];
				//T7_z=0.1F*global::mert_z[i];
				T7_dx=0.1F*dx;
				T7_dy=0.1F*dy;
				T7_dz=0.1F*dz;
				T7_ddx=0.1F*ddx;
				T7_ddy=0.1F*ddy;
				T7_ddz=0.1F*ddz;


				T7_[3]=global::kt[i].x;
				T7_[4]=global::kt[i].y;
				T7_[5]=global::kt[i].z;
				T7_[6]=global::kn[i].x;
				T7_[7]=global::kn[i].y;
				T7_[8]=global::kn[i].z;
				T7_[9]=global::kb[i].x;
				T7_[10]=global::kb[i].y;
				T7_[11]=global::kb[i].z;
				T7_[12]=gorb;

				break;
			case 14:
				pontosit(14, pdb, &x,  &y, &z, &dx,  &dy, &dz, &ddx,  &ddy, &ddz, &gorb);
				T8_x=0.1F*x;
				T8_y=0.1F*y;
				T8_z=0.1F*z;
				//T8_x=0.1F*global::mert_x[i];
				//T8_y=0.1F*global::mert_y[i];
				//T8_z=0.1F*global::mert_z[i];

				T8_dx=0.1F*dx;
				T8_dy=0.1F*dy;
				T8_dz=0.1F*dz;
				T8_ddx=0.1F*ddx;
				T8_ddy=0.1F*ddy;
				T8_ddz=0.1F*ddz;

				T8_[3]=global::kt[i].x;
				T8_[4]=global::kt[i].y;
				T8_[5]=global::kt[i].z;
				T8_[6]=global::kn[i].x;
				T8_[7]=global::kn[i].y;
				T8_[8]=global::kn[i].z;
				T8_[9]=global::kb[i].x;
				T8_[10]=global::kb[i].y;
				T8_[11]=global::kb[i].z;
				T8_[12]=gorb;
				break;
			case 15:
				pontosit(15, pdb, &x,  &y, &z, &dx,  &dy, &dz, &ddx,  &ddy, &ddz, &gorb);
				T9_x=0.1F*x;
				T9_y=0.1F*y;
				T9_z=0.1F*z;
				//T9_x=0.1F*global::mert_x[i];
				//T9_y=0.1F*global::mert_y[i];
				//T9_z=0.1F*global::mert_z[i];

				T9_dx=0.1F*dx;
				T9_dy=0.1F*dy;
				T9_dz=0.1F*dz;
				T9_ddx=0.1F*ddx;
				T9_ddy=0.1F*ddy;
				T9_ddz=0.1F*ddz;

				T9_[3]=global::kt[i].x;
				T9_[4]=global::kt[i].y;
				T9_[5]=global::kt[i].z;
				T9_[6]=global::kn[i].x;
				T9_[7]=global::kn[i].y;
				T9_[8]=global::kn[i].z;
				T9_[9]=global::kb[i].x;
				T9_[10]=global::kb[i].y;
				T9_[11]=global::kb[i].z;
				T9_[12]=gorb;
				break;
			case 16:
				pontosit(16, pdb, &x,  &y, &z, &dx,  &dy, &dz, &ddx,  &ddy, &ddz, &gorb);
				T10_x=0.1F*x;
				T10_y=0.1F*y;
				T10_z=0.1F*z;
				//T10_x=0.1F*global::mert_x[i];
				//T10_y=0.1F*global::mert_y[i];
				//T10_z=0.1F*global::mert_z[i];
				T10_dx=0.1F*dx;
				T10_dy=0.1F*dy;
				T10_dz=0.1F*dz;
				T10_ddx=0.1F*ddx;
				T10_ddy=0.1F*ddy;
				T10_ddz=0.1F*ddz;


				T10_[3]=global::kt[i].x;
				T10_[4]=global::kt[i].y;
				T10_[5]=global::kt[i].z;
				T10_[6]=global::kn[i].x;
				T10_[7]=global::kn[i].y;
				T10_[8]=global::kn[i].z;
				T10_[9]=global::kb[i].x;
				T10_[10]=global::kb[i].y;
				T10_[11]=global::kb[i].z;
				T10_[12]=gorb;
				break;
			case 17:
				pontosit(17, pdb, &x,  &y, &z, &dx,  &dy, &dz, &ddx,  &ddy, &ddz, &gorb);
				T11_x=0.1F*x;
				T11_y=0.1F*y;
				T11_z=0.1F*z;
				//T11_x=0.1F*global::mert_x[i];
				//T11_y=0.1F*global::mert_y[i];
				//T11_z=0.1F*global::mert_z[i];

				T11_dx=0.1F*dx;
				T11_dy=0.1F*dy;
				T11_dz=0.1F*dz;
				T11_ddx=0.1F*ddx;
				T11_ddy=0.1F*ddy;
				T11_ddz=0.1F*ddz;

				T11_[3]=global::kt[i].x;
				T11_[4]=global::kt[i].y;
				T11_[5]=global::kt[i].z;
				T11_[6]=global::kn[i].x;
				T11_[7]=global::kn[i].y;
				T11_[8]=global::kn[i].z;
				T11_[9]=global::kb[i].x;
				T11_[10]=global::kb[i].y;
				T11_[11]=global::kb[i].z;
				T11_[12]=gorb;

				break;
			case 18:
				pontosit(18, pdb, &x,  &y, &z, &dx,  &dy, &dz, &ddx,  &ddy, &ddz, &gorb);
				T12_x=0.1F*x;
				T12_y=0.1F*y;
				T12_z=0.1F*z;
				//T12_x=0.1F*global::mert_x[i];
				//T12_y=0.1F*global::mert_y[i];
				//T12_z=0.1F*global::mert_z[i];

				T12_dx=0.1F*dx;
				T12_dy=0.1F*dy;
				T12_dz=0.1F*dz;
				T12_ddx=0.1F*ddx;
				T12_ddy=0.1F*ddy;
				T12_ddz=0.1F*ddz;

				T12_[3]=global::kt[i].x;
				T12_[4]=global::kt[i].y;
				T12_[5]=global::kt[i].z;
				T12_[6]=global::kn[i].x;
				T12_[7]=global::kn[i].y;
				T12_[8]=global::kn[i].z;
				T12_[9]=global::kb[i].x;
				T12_[10]=global::kb[i].y;
				T12_[11]=global::kb[i].z;
				T12_[12]=gorb;

				break;
			case 19:
				pontosit(19, pdb, &x,  &y, &z, &dx,  &dy, &dz, &ddx,  &ddy, &ddz, &gorb);
				L1_x=0.1F*x;
				L1_y=0.1F*y;
				L1_z=0.1F*z;
				//L1_x=0.1F*global::mert_x[i];
				//L1_y=0.1F*global::mert_y[i];
				//L1_z=0.1F*global::mert_z[i];

				L1_dx=0.1F*dx;
				L1_dy=0.1F*dy;
				L1_dz=0.1F*dz;
				L1_ddx=0.1F*ddx;
				L1_ddy=0.1F*ddy;
				L1_ddz=0.1F*ddz;


				L1_[3]=global::kt[i].x;
				L1_[4]=global::kt[i].y;
				L1_[5]=global::kt[i].z;
				L1_[6]=global::kn[i].x;
				L1_[7]=global::kn[i].y;
				L1_[8]=global::kn[i].z;
				L1_[9]=global::kb[i].x;
				L1_[10]=global::kb[i].y;
				L1_[11]=global::kb[i].z;
				L1_[12]=gorb;

				break;
			case 20:
				pontosit(20, pdb, &x,  &y, &z, &dx,  &dy, &dz, &ddx,  &ddy, &ddz, &gorb);
				L2_x=0.1F*x;
				L2_y=0.1F*y;
				L2_z=0.1F*z;
				//L2_x=0.1F*global::mert_x[i];
				//L2_y=0.1F*global::mert_y[i];
				//L2_z=0.1F*global::mert_z[i];

				L2_dx=0.1F*dx;
				L2_dy=0.1F*dy;
				L2_dz=0.1F*dz;
				L2_ddx=0.1F*ddx;
				L2_ddy=0.1F*ddy;
				L2_ddz=0.1F*ddz;


				L2_[3]=global::kt[i].x;
				L2_[4]=global::kt[i].y;
				L2_[5]=global::kt[i].z;
				L2_[6]=global::kn[i].x;
				L2_[7]=global::kn[i].y;
				L2_[8]=global::kn[i].z;
				L2_[9]=global::kb[i].x;
				L2_[10]=global::kb[i].y;
				L2_[11]=global::kb[i].z;
				L2_[12]=gorb;

				break;
			case 21:
				pontosit(21, pdb, &x,  &y, &z, &dx,  &dy, &dz, &ddx,  &ddy, &ddz, &gorb);
				L3_x=0.1F*x;
				L3_y=0.1F*y;
				L3_z=0.1F*z;
				//L3_x=0.1F*global::mert_x[i];
				//L3_y=0.1F*global::mert_y[i];
				//L3_z=0.1F*global::mert_z[i];

				L3_dx=0.1F*dx;
				L3_dy=0.1F*dy;
				L3_dz=0.1F*dz;
				L3_ddx=0.1F*ddx;
				L3_ddy=0.1F*ddy;
				L3_ddz=0.1F*ddz;


				L3_[3]=global::kt[i].x;
				L3_[4]=global::kt[i].y;
				L3_[5]=global::kt[i].z;
				L3_[6]=global::kn[i].x;
				L3_[7]=global::kn[i].y;
				L3_[8]=global::kn[i].z;
				L3_[9]=global::kb[i].x;
				L3_[10]=global::kb[i].y;
				L3_[11]=global::kb[i].z;
				L3_[12]=gorb;


				break;
			case 22:
				pontosit(22, pdb, &x,  &y, &z, &dx,  &dy, &dz, &ddx,  &ddy, &ddz, &gorb);
				L4_x=0.1F*x;
				L4_y=0.1F*y;
				L4_z=0.1F*z;
				//L4_x=0.1F*global::mert_x[i];
				//L4_y=0.1F*global::mert_y[i];
				//L4_z=0.1F*global::mert_z[i];
				L4_dx=0.1F*dx;
				L4_dy=0.1F*dy;
				L4_dz=0.1F*dz;
				L4_ddx=0.1F*ddx;
				L4_ddy=0.1F*ddy;
				L4_ddz=0.1F*ddz;


				L4_[3]=global::kt[i].x;
				L4_[4]=global::kt[i].y;
				L4_[5]=global::kt[i].z;
				L4_[6]=global::kn[i].x;
				L4_[7]=global::kn[i].y;
				L4_[8]=global::kn[i].z;
				L4_[9]=global::kb[i].x;
				L4_[10]=global::kb[i].y;
				L4_[11]=global::kb[i].z;
				L4_[12]=gorb;

				break;
			}
			csig++;
		}
	}
	double x,y,z, dx, dy, dz, ddx, ddy, ddz, gorb;
	pontosit(23, pdb, &x,  &y, &z, &dx,  &dy, &dz, &ddx,  &ddy, &ddz, &gorb);
	L5_x=0.1F*global::mert_x[mp-1];
	L5_y=0.1F*global::mert_y[mp-1];
	L5_z=0.1F*global::mert_z[mp-1];
	
	L5_dx=0.1F*dx;
	L5_dy=0.1F*dy;
	L5_dz=0.1F*dz;
	L5_ddx=0.1F*ddx;
	L5_ddy=0.1F*ddy;
	L5_ddz=0.1F*ddz;

	L5_[3]=global::kt[mp-1].x;
	L5_[4]=global::kt[mp-1].y;
	L5_[5]=global::kt[mp-1].z;
	L5_[6]=global::kn[mp-1].x;
	L5_[7]=global::kn[mp-1].y;
	L5_[8]=global::kn[mp-1].z;
	L5_[9]=global::kb[mp-1].x;
	L5_[10]=global::kb[mp-1].y;
	L5_[11]=global::kb[mp-1].z;
	L5_[12]=gorb;

}

void gerinckiki(Graphics ^ g, float w, float h, int pdb, int melyik, int kiv, System::Windows::Forms::DataGridView ^ dgv, System::Windows::Forms::DataGridView ^ dgr)
{
			Pen ^ p = gcnew Pen(Color::Black);
			double xx,yy,zz;
			float xk = w/4;
			float yk=0.1*h;   
			float yv=0.9*h;
			float szor=0.8*h/(-(L5_z-C1_z)); 
			float vsz=0.5*szor;
			float d=w/40;
			double x,y,z, dx, dy, dz, ddx, ddy, ddz;



			dgv->Rows->Clear();
			dgr->Rows->Clear();
			//sx=String::Format("{0, 8:f2}", xh);
	 		//sy=String::Format("{0, 8:f2}", yh);
			//sz=String::Format("{0, 8:f2}", zh);


			const double pi=3.14159;

			if (!global::danka)
			{
				g->DrawRectangle(p,xk+vsz*C1_x-d/2,yk+szor*(-C1_z)-d/2,d,d);
				dgv->Rows->Add( "C1", String::Format("{0, 8:f2}", 0));
				dgr->Rows->Add(String::Format("{0, 8:f2}", 0));

				if (melyik==1)
				{
					g->DrawRectangle(p,xk+vsz*C2_x-d/2,yk+szor*(-C2_z)-d/2,d,d);
					dgv->Rows->Add( "C2", String::Format("{0, 8:f2}",C2_[6]*sqrt(pow(C2_ddx,2)+pow(C2_ddy,2)+pow(C2_ddz,2)))); 
				}
				else 
				{
					g->DrawRectangle(p,xk-vsz*C2_y-d/2,yk+szor*(-C2_z)-d/2,d,d);
					dgv->Rows->Add( "C2", String::Format("{0, 8:f2}",C2_[7]*sqrt(pow(C2_ddx,2)+pow(C2_ddy,2)+pow(C2_ddz,2))));
				}
				dgr->Rows->Add(String::Format("{0, 8:f2}",C2_[12])); 
				//dgr->Rows->Add(String::Format("{0, 8:f2}",90-acos(C2_[9])*180/pi)); 
				//dgr->Rows->Add(String::Format("{0, 8:f2}",atan(C2_[7]/C2_[6])*180/pi)); 
				if (melyik==1)
				{
					g->DrawRectangle(p,xk+vsz*C3_x-d/2,yk+szor*(-C3_z)-d/2,d,d);
					dgv->Rows->Add( "C3", String::Format("{0, 8:f2}",C3_[6]*sqrt(pow(C3_ddx,2)+pow(C3_ddy,2)+pow(C3_ddz,2)))); 
				}
				else
				{
					g->DrawRectangle(p,xk-vsz*C3_y-d/2,yk+szor*(-C3_z)-d/2,d,d);
					dgv->Rows->Add( "C3", String::Format("{0, 8:f2}",C3_[7]*sqrt(pow(C3_ddx,2)+pow(C3_ddy,2)+pow(C3_ddz,2)))); 
				}
				dgr->Rows->Add(String::Format("{0, 8:f2}",C3_[12])); 
				//dgr->Rows->Add(String::Format("{0, 8:f2}",90-acos(C3_[9])*180/pi)); 

				//dgr->Rows->Add(String::Format("{0, 8:f2}",atan(C3_[7]/C3_[6])*180/pi)); 

				if (melyik==1)
				{
					g->DrawRectangle(p,xk+vsz*C4_x-d/2,yk+szor*(-C4_z)-d/2,d,d);
					dgv->Rows->Add( "C4", String::Format("{0, 8:f2}",C4_[6]*sqrt(pow(C4_ddx,2)+pow(C4_ddy,2)+pow(C4_ddz,2)))); 
				}
				else
				{
					g->DrawRectangle(p,xk-vsz*C4_y-d/2,yk+szor*(-C4_z)-d/2,d,d);
					dgv->Rows->Add( "C4", String::Format("{0, 8:f2}",C4_[7]*sqrt(pow(C4_ddx,2)+pow(C4_ddy,2)+pow(C4_ddz,2)))); 
				}
				dgr->Rows->Add(String::Format("{0, 8:f2}",C4_[12])); 
				//dgr->Rows->Add(String::Format("{0, 8:f2}",90-acos(C4_[9])*180/pi)); 
				//dgr->Rows->Add(String::Format("{0, 8:f2}",atan(C4_[7]/C4_[6])*180/pi)); 
				

				if (melyik==1)
				{
					g->DrawRectangle(p,xk+vsz*C5_x-d/2,yk+szor*(-C5_z)-d/2,d,d);
					dgv->Rows->Add( "C5", String::Format("{0, 8:f2}",C5_[6]*sqrt(pow(C5_ddx,2)+pow(C5_ddy,2)+pow(C5_ddz,2)))); 
				}
				else
				{
					g->DrawRectangle(p,xk-vsz*C5_y-d/2,yk+szor*(-C5_z)-d/2,d,d);
					dgv->Rows->Add( "C5", String::Format("{0, 8:f2}",C5_[7]*sqrt(pow(C5_ddx,2)+pow(C5_ddy,2)+pow(C5_ddz,2)))); 
				}
				dgr->Rows->Add(String::Format("{0, 8:f2}",C5_[12])); 
				//dgr->Rows->Add(String::Format("{0, 8:f2}",90-acos(C5_[9])*180/pi)); 
				//xx=C5_[6];
				//yy=C5_[7];
				//zz=C5_[8];
				//dgr->Rows->Add(String::Format("{0, 8:f2}",atan(C5_[7]/C5_[6])*180/pi)); 
			
				if (melyik==1)
				{
					g->DrawRectangle(p,xk+vsz*C6_x-d/2,yk+szor*(-C6_z)-d/2,d,d);
					dgv->Rows->Add( "C6", String::Format("{0, 8:f2}",C6_[6]*sqrt(pow(C6_ddx,2)+pow(C6_ddy,2)+pow(C6_ddz,2)))); 
				}
				else
				{
					g->DrawRectangle(p,xk-vsz*C6_y-d/2,yk+szor*(-C6_z)-d/2,d,d);
					dgv->Rows->Add( "C6", String::Format("{0, 8:f2}",C6_[7]*sqrt(pow(C6_ddx,2)+pow(C6_ddy,2)+pow(C6_ddz,2)))); 
				}
				dgr->Rows->Add(String::Format("{0, 8:f2}",C6_[12])); 
				//dgr->Rows->Add(String::Format("{0, 8:f2}",90-acos(C6_[9])*180/pi)); 
				//dgr->Rows->Add(String::Format("{0, 8:f2}",atan(C6_[7]/C6_[6])*180/pi)); 
			
				if (melyik==1)
				{
					g->DrawRectangle(p,xk+vsz*C7_x-d/2,yk+szor*(-C7_z)-d/2,d,d);
					dgv->Rows->Add( "C7", String::Format("{0, 8:f2}",C7_[6]*sqrt(pow(C7_ddx,2)+pow(C7_ddy,2)+pow(C7_ddz,2)))); 
				}
				else
				{
					g->DrawRectangle(p,xk-vsz*C7_y-d/2,yk+szor*(-C7_z)-d/2,d,d);
					dgv->Rows->Add( "C7", String::Format("{0, 8:f2}",C7_[7]*sqrt(pow(C7_ddx,2)+pow(C7_ddy,2)+pow(C7_ddz,2)))); 
				}
				dgr->Rows->Add(String::Format("{0, 8:f2}",C7_[12]));
				//dgr->Rows->Add(String::Format("{0, 8:f2}",90-acos(C7_[9])*180/pi)); 
				//dgr->Rows->Add(String::Format("{0, 8:f2}",atan(C7_[7]/C7_[6])*180/pi)); 

			}
			if (melyik==1)
			{
				g->DrawRectangle(p,xk+vsz*T1_x-d/2,yk+szor*(-T1_z)-d/2,d,d);
				dgv->Rows->Add( "T1", String::Format("{0, 8:f2}",T1_[6]*sqrt(pow(T1_ddx,2)+pow(T1_ddy,2)+pow(T1_ddz,2)))); 
			}
			else
			{
				g->DrawRectangle(p,xk-vsz*T1_y-d/2,yk+szor*(-T1_z)-d/2,d,d);
				dgv->Rows->Add( "T1", String::Format("{0, 8:f2}",T1_[7]*sqrt(pow(T1_ddx,2)+pow(T1_ddy,2)+pow(T1_ddz,2)))); 
			}
			dgr->Rows->Add(String::Format("{0, 8:f2}",T1_[12])); 

			if (melyik==1)
			{
				g->DrawRectangle(p,xk+vsz*T2_x-d/2,yk+szor*(-T2_z)-d/2,d,d);
				dgv->Rows->Add( "T2", String::Format("{0, 8:f2}",T2_[6]*sqrt(pow(T2_ddx,2)+pow(T2_ddy,2)+pow(T2_ddz,2)))); 
			}
			else
			{
				g->DrawRectangle(p,xk-vsz*T2_y-d/2,yk+szor*(-T2_z)-d/2,d,d);
				dgv->Rows->Add( "T2", String::Format("{0, 8:f2}",T2_[7]*sqrt(pow(T2_ddx,2)+pow(T2_ddy,2)+pow(T2_ddz,2)))); 
			}
			dgr->Rows->Add(String::Format("{0, 8:f2}",T2_[12])); 

			if (melyik==1)
			{
				g->DrawRectangle(p,xk+vsz*T3_x-d/2,yk+szor*(-T3_z)-d/2,d,d);
				dgv->Rows->Add( "T3", String::Format("{0, 8:f2}",T3_[6]*sqrt(pow(T3_ddx,2)+pow(T3_ddy,2)+pow(T3_ddz,2)))); 
			}
			else
			{
				g->DrawRectangle(p,xk-vsz*T3_y-d/2,yk+szor*(-T3_z)-d/2,d,d);
				dgv->Rows->Add( "T3", String::Format("{0, 8:f2}",T3_[7]*sqrt(pow(T3_ddx,2)+pow(T3_ddy,2)+pow(T3_ddz,2)))); 
			}
			dgr->Rows->Add(String::Format("{0, 8:f2}",T2_[12])); 

			if (melyik==1)
			{
				g->DrawRectangle(p,xk+vsz*T4_x-d/2,yk+szor*(-T4_z)-d/2,d,d);
				dgv->Rows->Add( "T4", String::Format("{0, 8:f2}",T4_[6]*sqrt(pow(T4_ddx,2)+pow(T4_ddy,2)+pow(T4_ddz,2)))); 
			}
			else
			{
				g->DrawRectangle(p,xk-vsz*T4_y-d/2,yk+szor*(-T4_z)-d/2,d,d);
				dgv->Rows->Add( "T4", String::Format("{0, 8:f2}",T4_[7]*sqrt(pow(T4_ddx,2)+pow(T4_ddy,2)+pow(T4_ddz,2)))); 
			}
			dgr->Rows->Add(String::Format("{0, 8:f2}",T4_[12])); 

			if (melyik==1)
			{
				g->DrawRectangle(p,xk+vsz*T5_x-d/2,yk+szor*(-T5_z)-d/2,d,d);
				dgv->Rows->Add( "T5", String::Format("{0, 8:f2}",T5_[6]*sqrt(pow(T5_ddx,2)+pow(T5_ddy,2)+pow(T5_ddz,2)))); 
			}
			else
			{
				g->DrawRectangle(p,xk-vsz*T5_y-d/2,yk+szor*(-T5_z)-d/2,d,d);
				dgv->Rows->Add( "T5", String::Format("{0, 8:f2}",T5_[7]*sqrt(pow(T5_ddx,2)+pow(T5_ddy,2)+pow(T5_ddz,2)))); 
			}
			dgr->Rows->Add(String::Format("{0, 8:f2}",T5_[12])); 

			if (melyik==1)
			{
				g->DrawRectangle(p,xk+vsz*T6_x-d/2,yk+szor*(-T6_z)-d/2,d,d);
				dgv->Rows->Add( "T6", String::Format("{0, 8:f2}",T6_[6]*sqrt(pow(T6_ddx,2)+pow(T6_ddy,2)+pow(T6_ddz,2)))); 
			}
			else
			{
				g->DrawRectangle(p,xk-vsz*T6_y-d/2,yk+szor*(-T6_z)-d/2,d,d);
				dgv->Rows->Add( "T6", String::Format("{0, 8:f2}",T6_[7]*sqrt(pow(T6_ddx,2)+pow(T6_ddy,2)+pow(T6_ddz,2)))); 
			}
			dgr->Rows->Add(String::Format("{0, 8:f2}",T6_[12])); 

			if (melyik==1)
			{
				g->DrawRectangle(p,xk+vsz*T7_x-d/2,yk+szor*(-T7_z)-d/2,d,d);
				dgv->Rows->Add( "T7", String::Format("{0, 8:f2}",T7_[6]*sqrt(pow(T7_ddx,2)+pow(T7_ddy,2)+pow(T7_ddz,2)))); 
			}
			else
			{
				g->DrawRectangle(p,xk-vsz*T7_y-d/2,yk+szor*(-T7_z)-d/2,d,d);
				dgv->Rows->Add( "T7", String::Format("{0, 8:f2}",T7_[7]*sqrt(pow(T7_ddx,2)+pow(T7_ddy,2)+pow(T7_ddz,2)))); 
			}
			dgr->Rows->Add(String::Format("{0, 8:f2}",T7_[12])); 

			if (melyik==1)
			{
				g->DrawRectangle(p,xk+vsz*T8_x-d/2,yk+szor*(-T8_z)-d/2,d,d);
				dgv->Rows->Add( "T8", String::Format("{0, 8:f2}",T8_[6]*sqrt(pow(T8_ddx,2)+pow(T8_ddy,2)+pow(T8_ddz,2)))); 
			}
			else
			{
				g->DrawRectangle(p,xk-vsz*T8_y-d/2,yk+szor*(-T8_z)-d/2,d,d);
				dgv->Rows->Add( "T8", String::Format("{0, 8:f2}",T8_[7]*sqrt(pow(T8_ddx,2)+pow(T8_ddy,2)+pow(T8_ddz,2)))); 
			}
			dgr->Rows->Add(String::Format("{0, 8:f2}",T8_[12])); 
			if (melyik==1)
			{
				g->DrawRectangle(p,xk+vsz*T9_x-d/2,yk+szor*(-T9_z)-d/2,d,d);
				dgv->Rows->Add( "T9", String::Format("{0, 8:f2}",T9_[6]*sqrt(pow(T9_ddx,2)+pow(T9_ddy,2)+pow(T9_ddz,2)))); 
			}
			else
			{
				g->DrawRectangle(p,xk-vsz*T9_y-d/2,yk+szor*(-T9_z)-d/2,d,d);
				dgv->Rows->Add( "T9", String::Format("{0, 8:f2}",T9_[7]*sqrt(pow(T9_ddx,2)+pow(T9_ddy,2)+pow(T9_ddz,2)))); 
			}
			dgr->Rows->Add(String::Format("{0, 8:f2}",T9_[12])); 
			if (melyik==1)
			{
				g->DrawRectangle(p,xk+vsz*T10_x-d/2,yk+szor*(-T10_z)-d/2,d,d);
				dgv->Rows->Add( "T10", String::Format("{0, 8:f2}",T10_[6]*sqrt(pow(T10_ddx,2)+pow(T10_ddy,2)+pow(T10_ddz,2)))); 
			}
			else
			{
				g->DrawRectangle(p,xk-vsz*T10_y-d/2,yk+szor*(-T10_z)-d/2,d,d);
				dgv->Rows->Add( "T10", String::Format("{0, 8:f2}",T10_[7]*sqrt(pow(T10_ddx,2)+pow(T10_ddy,2)+pow(T10_ddz,2)))); 
			}
			dgr->Rows->Add(String::Format("{0, 8:f2}",T10_[12])); 

			if (melyik==1)
			{
				g->DrawRectangle(p,xk+vsz*T11_x-d/2,yk+szor*(-T11_z)-d/2,d,d);
				dgv->Rows->Add( "T11", String::Format("{0, 8:f2}",T11_[6]*sqrt(pow(T11_ddx,2)+pow(T11_ddy,2)+pow(T11_ddz,2)))); 
			}
			else
			{
				g->DrawRectangle(p,xk-vsz*T11_y-d/2,yk+szor*(-T11_z)-d/2,d,d);
				dgv->Rows->Add( "T11", String::Format("{0, 8:f2}",T11_[7]*sqrt(pow(T11_ddx,2)+pow(T11_ddy,2)+pow(T11_ddz,2)))); 
			}
			dgr->Rows->Add(String::Format("{0, 8:f2}",T11_[12])); 

			if (melyik==1)
			{
				g->DrawRectangle(p,xk+vsz*T12_x-d/2,yk+szor*(-T12_z)-d/2,d,d);
				dgv->Rows->Add( "T12", String::Format("{0, 8:f2}",T12_[6]*sqrt(pow(T12_ddx,2)+pow(T12_ddy,2)+pow(T12_ddz,2)))); 
			}
			else
			{
				g->DrawRectangle(p,xk-vsz*T12_y-d/2,yk+szor*(-T12_z)-d/2,d,d);
				dgv->Rows->Add( "T12", String::Format("{0, 8:f2}",T12_[7]*sqrt(pow(T12_ddx,2)+pow(T12_ddy,2)+pow(T12_ddz,2)))); 
			}
			dgr->Rows->Add(String::Format("{0, 8:f2}",T12_[12])); 


			if (melyik==1)
			{
				g->DrawRectangle(p,xk+vsz*L1_x-d/2,yk+szor*(-L1_z)-d/2,d,d);
				dgv->Rows->Add( "L1", String::Format("{0, 8:f2}",L1_[6]*sqrt(pow(L1_ddx,2)+pow(L1_ddy,2)+pow(L1_ddz,2)))); 
			}
			else
			{
				g->DrawRectangle(p,xk-vsz*L1_y-d/2,yk+szor*(-L1_z)-d/2,d,d);
				dgv->Rows->Add( "L1", String::Format("{0, 8:f2}",L1_[7]*sqrt(pow(L1_ddx,2)+pow(L1_ddy,2)+pow(L1_ddz,2)))); 
			}
			dgr->Rows->Add(String::Format("{0, 8:f2}",L1_[12])); 

			if (melyik==1)
			{
				g->DrawRectangle(p,xk+vsz*L2_x-d/2,yk+szor*(-L2_z)-d/2,d,d);
				dgv->Rows->Add( "L2", String::Format("{0, 8:f2}",L2_[6]*sqrt(pow(L2_ddx,2)+pow(L2_ddy,2)+pow(L2_ddz,2)))); 
			}
			else
			{
				g->DrawRectangle(p,xk-vsz*L2_y-d/2,yk+szor*(-L2_z)-d/2,d,d);
				dgv->Rows->Add( "L2",String::Format("{0, 8:f2}",L2_[7]*sqrt(pow(L2_ddx,2)+pow(L2_ddy,2)+pow(L2_ddz,2)))); 
			}
			dgr->Rows->Add(String::Format("{0, 8:f2}",L2_[12])); 

			if (melyik==1)
			{
				g->DrawRectangle(p,xk+vsz*L3_x-d/2,yk+szor*(-L3_z)-d/2,d,d);
				dgv->Rows->Add( "L3", String::Format("{0, 8:f2}",L3_[6]*sqrt(pow(L3_ddx,2)+pow(L3_ddy,2)+pow(L3_ddz,2)))); 
			}
			else
			{
				g->DrawRectangle(p,xk-vsz*L3_y-d/2,yk+szor*(-L3_z)-d/2,d,d);
				dgv->Rows->Add( "L3", String::Format("{0, 8:f2}",L3_[7]*sqrt(pow(L3_ddx,2)+pow(L3_ddy,2)+pow(L3_ddz,2)))); 
			}
			dgr->Rows->Add(String::Format("{0, 8:f2}",L3_[12])); 

			if (melyik==1)
			{
				g->DrawRectangle(p,xk+vsz*L4_x-d/2,yk+szor*(-L4_z)-d/2,d,d);
				dgv->Rows->Add( "L4", String::Format("{0, 8:f2}",L4_[6]*sqrt(pow(L4_ddx,2)+pow(L4_ddy,2)+pow(L4_ddz,2)))); 
			}
			else
			{
				g->DrawRectangle(p,xk-vsz*L4_y-d/2,yk+szor*(-L4_z)-d/2,d,d);
				dgv->Rows->Add( "L4", String::Format("{0, 8:f2}",L4_[7]*sqrt(pow(L4_ddx,2)+pow(L4_ddy,2)+pow(L4_ddz,2)))); 
			}
			dgr->Rows->Add(String::Format("{0, 8:f2}",L4_[12])); 
			if (melyik==1)
			{
				g->DrawRectangle(p,xk+vsz*L5_x-d/2,yk+szor*(-L5_z)-d/2,d,d);
				dgv->Rows->Add( "L5", String::Format("{0, 8:f2}",L5_[6]*sqrt(pow(L5_ddx,2)+pow(L5_ddy,2)+pow(L5_ddz,2)))); 
			}
			else
			{
				g->DrawRectangle(p,xk-vsz*L5_y-d/2,yk+szor*(-L5_z)-d/2,d,d);
				dgv->Rows->Add( "L5", String::Format("{0, 8:f2}",L5_[7]*sqrt(pow(L5_ddx,2)+pow(L5_ddy,2)+pow(L5_ddz,2)))); 
			}
			dgr->Rows->Add(String::Format("{0, 8:f2}",0));//L5_[12])); 


			if (kiv!=0)
			{
				Pen ^ pi = gcnew Pen(Color::Red);
				if (melyik==1) 
				{
					g->DrawRectangle(pi,xk+vsz*0.1*global::points[kiv].xr-d/2,yk+szor*(-0.1*global::points[kiv].zr)-d/2,d,d);
				}
				else
				{
					g->DrawRectangle(pi,xk-vsz*0.1*global::points[kiv].yr-d/2,yk+szor*(-0.1*global::points[kiv].zr)-d/2,d,d);
				}
			}
			

}