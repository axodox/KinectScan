#ifndef _H
#define _H
    struct g3d
        {
		public:
			double ivh;
			double xr;
            double yr;
            double zr;
			double dx;
            double dy;
            double dz;
			double ddx;
            double ddy;
            double ddz;
			double dddx;
            double dddy;
            double dddz;
        };
    struct p3d
        {
		public:
			float x;
            float y;
            float z;
		};

public ref class global{
public:
	static bool hatgerinc=true; 
	static bool danka=false; 
	static bool vangerinc=false;

    static g3d * points=NULL;
    static p3d * kt=NULL;
    static p3d * kb=NULL;
    static p3d * kn=NULL;





	static int mertp;
	static double iv_alap=0;
	static double iv_mert=0;
	static double gsc=1;
	static array <double>^ csig_iv=gcnew array<double>(24);
	static array <double>^ mert_iv=gcnew array<double>(10000);
	static array <double>^ mert_x=gcnew array<double>(10000);
	static array <double>^ mert_y=gcnew array<double>(10000);
	static array <double>^ mert_z=gcnew array<double>(10000);
};
void gerincki();
void gerinckiki(Graphics ^ g, float w, float h, int pdb, int melyik, int kiv, System::Windows::Forms::DataGridView ^ dgv, System::Windows::Forms::DataGridView ^ dgr);
double alap_ivhossz();
void passz(int mp, int pdb);
#endif


