#pragma once
#include "NameIn1.h"
#include <stdio.h>
#include <math.h>
#include "Reg.h"
#include "gerincrajz.h"

#using <mscorlib.dll>

using namespace System;
using namespace System::Globalization;
using namespace System::Threading;

using namespace Reg;


namespace geusbcplus {

	double csig_iv[24];

	using namespace System;
	using namespace System::ComponentModel;
	using namespace System::Collections;
	using namespace System::Windows::Forms;
	using namespace System::Data;
	using namespace System::Drawing;
	using namespace System::IO;

	/// <summary>
	/// Summary for Form1
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>


	static String ^ ParseCoord(String ^ & s, String ^ & elv) {
            // Szétszed
            int hol;
            hol = s->IndexOf(elv);
            if (hol > 0) {
                String ^ s1 = s->Substring(0, hol);
                s = s->Substring(hol + 1, s->Length - hol - 1);
                return s1;
            }
            else {
                String ^ s1 = s;
                s = "";
                return s1;
            }
        }


	public ref class Form1 : public System::Windows::Forms::Form
	{
	static float zoomszog=100;
	static float tav;
	static float nezszog=-90.0;
	static float hatul=400.0;
	static float testszog=0;//-45;
	static float szemmag=-350;//150;//90
	static int db=0;
    static int point_db;
	String ^ file_name;
	StreamWriter ^ fw0;
	static String ^ csk=gcnew String(";"); 
	static String ^ csc=gcnew String(";");
	static String ^ outputdir=gcnew String("c:\\scan\\");
	static String ^ datedir;
	static int XE=-1000;
	static int YE=-1000;
	static String ^ sx="";
	static String ^ sy="";
	static String ^ sz="";
	static int kiv=0;

	DateTime^ datumido;
	private: System::Windows::Forms::ToolStripButton^  OpenB;
	private: System::Windows::Forms::OpenFileDialog^  openFileDialog1;
	private: System::Windows::Forms::Panel^  panel_r;
	private: System::Windows::Forms::Panel^  panel_l;
	private: System::Windows::Forms::DataGridView^  dgxyz;
	private: System::Windows::Forms::DataGridViewTextBoxColumn^  x_c;
	private: System::Windows::Forms::DataGridViewTextBoxColumn^  y_c;
	private: System::Windows::Forms::DataGridViewTextBoxColumn^  z_c;
	private: System::Windows::Forms::ToolStripLabel^  toolStripLabel1;
	private: System::Windows::Forms::ToolStripSeparator^  toolStripSeparator2;
	private: System::Windows::Forms::ToolStripButton^  ger_vi;
	private: System::Windows::Forms::TabControl^  tabControl1;


	private: System::Windows::Forms::TabPage^  tabPage2;
	private: System::Windows::Forms::DataGridView^  dGVgh;
	private: System::Windows::Forms::DataGridViewTextBoxColumn^  dataGridViewTextBoxColumn1;
	private: System::Windows::Forms::DataGridViewTextBoxColumn^  dataGridViewTextBoxColumn2;

	private: System::Windows::Forms::DataGridViewTextBoxColumn^  G;
	private: System::Windows::Forms::DataGridViewTextBoxColumn^  Cs;
	private: System::Windows::Forms::TabPage^  tabPage1;
	private: System::Windows::Forms::DataGridView^  dGVgo;


	private: System::Windows::Forms::DataGridView^  dGVr;
	private: System::Windows::Forms::DataGridViewTextBoxColumn^  Column1;
	private: System::Windows::Forms::DataGridViewTextBoxColumn^  Cso;
	private: System::Windows::Forms::DataGridViewTextBoxColumn^  Go;




































			 StreamWriter ^sw; // output-hoz.

	System::String^ datum2string(DateTime^ t) {
		String^ s=gcnew String("");
		s=t->Year.ToString("0000")+t->Month.ToString("00")+t->Day.ToString("00");
		s+="-"+t->Hour.ToString("00")+t->Minute.ToString("00")+t->Second.ToString("00");
		return s;
	}
	bool filename_ok(String^ s) {
		bool ok=true;
		int i;
		wchar_t c;
		for (i=0; i<s->Length;i++) {
			c=s[i];
			if ( (c<'0') || // before 0
				 ((c>'9') && (c<'A')) || // after 9 before A
				((c>'Z') && (c<'a')) || // after Z before a
				(c>'z')) ok=false; // after z (after eight)
		}
	return ok;
	}
	float vh2(const float x1, const float y1, const float x2, const float y2)
	{
		return Math::Sqrt(Math::Pow(x1-x2,2)+Math::Pow(y1-y2,2));
	}


	private: System::Windows::Forms::ToolStrip^  toolStrip1;
	private: System::Windows::Forms::ToolStripButton^  StartB;
	private: System::Windows::Forms::ToolStripLabel^  Kiiro;

	private: System::Windows::Forms::ToolStripSeparator^  toolStripSeparator1;

	private: System::Windows::Forms::ToolStripButton^  jForg;
	private: System::Windows::Forms::ToolStripButton^  bForg;
	private: System::Windows::Forms::ToolStripButton^  Zoomout;
	private: System::Windows::Forms::ToolStripButton^  toolStripButton2;
	private: System::Windows::Forms::ToolStripButton^  toolStripButton1;
	private: System::Windows::Forms::ToolStripButton^  toolStripButton3;
	private: System::Windows::Forms::ToolStripButton^  Zoomin;




	
	public:
		Form1(void)
		{
			InitializeComponent();
			//
			//TODO: Add the constructor code here
			//
		}
		HDC m_hDC;
		HWND hwnd;
		HGLRC m_hglrc;

	protected:
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		~Form1()
		{
			if (components)
			{
				delete components;
			}
		}

		void Kocka(double pzPontx, double pzPonty, double pzPontz, double elhossz, int szin) {
			glPolygonMode(GL_FRONT_AND_BACK,GL_FILL);
			glDisable(GL_TEXTURE_2D);
			switch (szin) {
				case 0: 
					glColor3d(0.85,0.85,0.5);
//					glColor3d(1,0,0);    //piros - semmi
					break;
				case 1:
					glColor3d(1,0,0);    //piros - semmi
//					glColor3d(0,1,0);    //zold - jobblab
					break;
				case 2:
					glColor3d(1,1,0);    //sarga - ballab
					break;
				case 3:
					glColor3d(0,0,1);	 // vilkek - jobbkez
					break;
				case 4:
					glColor3d(0,0.5,0);  // szold - balkez
					break;
				case 5:
					glColor3d(0,0,0.5);  // kék - torzs
					break;
				case 6:
					glColor3d(0.5,0.25,0.25);  //smalyva - jobbvall
					break;
				case 7:
					glColor3d(0.5,0.5,0);  //ssarga - balvall
					break;
				case 8:
					glColor3d(0.75,0.75,0.75);  //szürke - jobbeleje
					break;
				case 9:
					glColor3d(1,0,1);  // pink - jobbhatul
					break;
				case 10:
					glColor3d(0,0.5,1); // vcian - baleleje
					break;
				case 11:
					glColor3d(0,1,1);  // cian - balhatul
					break;
				case 12:
					glColor3d(0.5,0,0.5); // lila - nyak
					break;
				case 13:
					glColor3d(0.5,0.5,0); // libafos - arc
					break;
				case 14:
					glColor3d(0.25,0,0); // barna - haj
					break;
				case 15:
					glColor3d(1,0.5,0);  //narancs- honaljalatt asszim
					break;
				case 20:
					glColor3d(1,1,1);  // csont - fehér
					break;
			}
			glPointSize((GLfloat)elhossz);
			glBegin(GL_POINTS);
				glVertex3d(pzPontx,pzPonty,pzPontz);
			glEnd();
			glPointSize(1);
//			gluSphere (gomb,10,6,6);

		}


		GLint MySetPixelFormat(HDC hdc)
		{
			static	PIXELFORMATDESCRIPTOR pfd=				// pfd Tells Windows How We Want Things To Be
				{
					sizeof(PIXELFORMATDESCRIPTOR),				// Size Of This Pixel Format Descriptor
					1,											// Version Number
					PFD_DRAW_TO_WINDOW |						// Format Must Support Window
					PFD_SUPPORT_OPENGL |						// Format Must Support OpenGL
					PFD_DOUBLEBUFFER,							// Must Support Double Buffering
					PFD_TYPE_RGBA,								// Request An RGBA Format
					32,											// Select Our Color Depth
					0, 0, 0, 0, 0, 0,							// Color Bits Ignored
					0,											// No Alpha Buffer
					0,											// Shift Bit Ignored
					0,											// No Accumulation Buffer
					0, 0, 0, 0,									// Accumulation Bits Ignored
					32,											// 16Bit Z-Buffer (Depth Buffer)  
					0,											// No Stencil Buffer
					0,											// No Auxiliary Buffer
					PFD_MAIN_PLANE,								// Main Drawing Layer
					0,											// Reserved
					0, 0, 0										// Layer Masks Ignored
				};
			
			GLint  iPixelFormat; 
		 
			// get the device context's best, available pixel format match 
			if((iPixelFormat = ChoosePixelFormat(hdc, &pfd)) == 0)
			{
				MessageBox::Show("ChoosePixelFormat Failed");
				return 0;
			}
			 
			// make that match the device context's current pixel format 
			if(SetPixelFormat(hdc, iPixelFormat, &pfd) == FALSE)
			{
				MessageBox::Show("SetPixelFormat Failed");
				return 0;
			}

			if((m_hglrc = wglCreateContext(m_hDC)) == NULL)
			{
				MessageBox::Show("wglCreateContext Failed");
				return 0;
			}

			if((wglMakeCurrent(m_hDC, m_hglrc)) == NULL)
			{
				MessageBox::Show("wglMakeCurrent Failed");
				return 0;
			}
			return 1;
		}

		bool InitGL(GLvoid)										// All setup for opengl goes here
		{
			glShadeModel(GL_SMOOTH);							// Enable smooth shading
			glClearColor(1.0f, 1.0f, 1.0f, 0.5f);				// Black background
			glClearDepth(1.0f);									// Depth buffer setup
			glEnable(GL_DEPTH_TEST);							// Enables depth testing
			glDepthFunc(GL_LEQUAL);								// The type of depth testing to do
			glHint(GL_PERSPECTIVE_CORRECTION_HINT, GL_NICEST);	// Really nice perspective calculations

		    // A default megvilágítás
//			glEnable(GL_LIGHTING);
//			glEnable(GL_LIGHT0);
		    // A felület normálvaktorainak számítása tükrözõdéshez
			glEnable(GL_AUTO_NORMAL);
			glEnable(GL_POLYGON_SMOOTH);
			glEnable(GL_NORMALIZE);
			return TRUE;										// Initialisation went ok
		}

	private: System::Windows::Forms::Timer^  timer1;
	private: System::IO::Ports::SerialPort^  serialPort1;
	private: System::Windows::Forms::StatusStrip^  statusStrip1;
	private: System::Windows::Forms::ToolStripStatusLabel^  toolStripStatusLabel1;

	private: System::ComponentModel::IContainer^  components;

	private:
		/// <summary>
		/// Required designer variable.
		/// </summary>

#pragma region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		void InitializeComponent(void)
		{
			this->components = (gcnew System::ComponentModel::Container());
			System::ComponentModel::ComponentResourceManager^  resources = (gcnew System::ComponentModel::ComponentResourceManager(Form1::typeid));
			this->timer1 = (gcnew System::Windows::Forms::Timer(this->components));
			this->serialPort1 = (gcnew System::IO::Ports::SerialPort(this->components));
			this->statusStrip1 = (gcnew System::Windows::Forms::StatusStrip());
			this->toolStripStatusLabel1 = (gcnew System::Windows::Forms::ToolStripStatusLabel());
			this->toolStrip1 = (gcnew System::Windows::Forms::ToolStrip());
			this->StartB = (gcnew System::Windows::Forms::ToolStripButton());
			this->OpenB = (gcnew System::Windows::Forms::ToolStripButton());
			this->Kiiro = (gcnew System::Windows::Forms::ToolStripLabel());
			this->toolStripSeparator1 = (gcnew System::Windows::Forms::ToolStripSeparator());
			this->jForg = (gcnew System::Windows::Forms::ToolStripButton());
			this->bForg = (gcnew System::Windows::Forms::ToolStripButton());
			this->Zoomin = (gcnew System::Windows::Forms::ToolStripButton());
			this->Zoomout = (gcnew System::Windows::Forms::ToolStripButton());
			this->toolStripButton2 = (gcnew System::Windows::Forms::ToolStripButton());
			this->toolStripButton3 = (gcnew System::Windows::Forms::ToolStripButton());
			this->toolStripButton1 = (gcnew System::Windows::Forms::ToolStripButton());
			this->toolStripLabel1 = (gcnew System::Windows::Forms::ToolStripLabel());
			this->toolStripSeparator2 = (gcnew System::Windows::Forms::ToolStripSeparator());
			this->ger_vi = (gcnew System::Windows::Forms::ToolStripButton());
			this->openFileDialog1 = (gcnew System::Windows::Forms::OpenFileDialog());
			this->panel_r = (gcnew System::Windows::Forms::Panel());
			this->dGVr = (gcnew System::Windows::Forms::DataGridView());
			this->tabControl1 = (gcnew System::Windows::Forms::TabControl());
			this->tabPage1 = (gcnew System::Windows::Forms::TabPage());
			this->dGVgo = (gcnew System::Windows::Forms::DataGridView());
			this->tabPage2 = (gcnew System::Windows::Forms::TabPage());
			this->dGVgh = (gcnew System::Windows::Forms::DataGridView());
			this->dataGridViewTextBoxColumn1 = (gcnew System::Windows::Forms::DataGridViewTextBoxColumn());
			this->dataGridViewTextBoxColumn2 = (gcnew System::Windows::Forms::DataGridViewTextBoxColumn());
			this->panel_l = (gcnew System::Windows::Forms::Panel());
			this->dgxyz = (gcnew System::Windows::Forms::DataGridView());
			this->x_c = (gcnew System::Windows::Forms::DataGridViewTextBoxColumn());
			this->y_c = (gcnew System::Windows::Forms::DataGridViewTextBoxColumn());
			this->z_c = (gcnew System::Windows::Forms::DataGridViewTextBoxColumn());
			this->G = (gcnew System::Windows::Forms::DataGridViewTextBoxColumn());
			this->Cs = (gcnew System::Windows::Forms::DataGridViewTextBoxColumn());
			this->Column1 = (gcnew System::Windows::Forms::DataGridViewTextBoxColumn());
			this->Cso = (gcnew System::Windows::Forms::DataGridViewTextBoxColumn());
			this->Go = (gcnew System::Windows::Forms::DataGridViewTextBoxColumn());
			this->statusStrip1->SuspendLayout();
			this->toolStrip1->SuspendLayout();
			this->panel_r->SuspendLayout();
			(cli::safe_cast<System::ComponentModel::ISupportInitialize^  >(this->dGVr))->BeginInit();
			this->tabControl1->SuspendLayout();
			this->tabPage1->SuspendLayout();
			(cli::safe_cast<System::ComponentModel::ISupportInitialize^  >(this->dGVgo))->BeginInit();
			this->tabPage2->SuspendLayout();
			(cli::safe_cast<System::ComponentModel::ISupportInitialize^  >(this->dGVgh))->BeginInit();
			this->panel_l->SuspendLayout();
			(cli::safe_cast<System::ComponentModel::ISupportInitialize^  >(this->dgxyz))->BeginInit();
			this->SuspendLayout();
			// 
			// timer1
			// 
			this->timer1->Tick += gcnew System::EventHandler(this, &Form1::timer1_Tick);
			// 
			// statusStrip1
			// 
			this->statusStrip1->Items->AddRange(gcnew cli::array< System::Windows::Forms::ToolStripItem^  >(1) {this->toolStripStatusLabel1});
			this->statusStrip1->Location = System::Drawing::Point(0, 244);
			this->statusStrip1->Name = L"statusStrip1";
			this->statusStrip1->Size = System::Drawing::Size(641, 22);
			this->statusStrip1->TabIndex = 0;
			this->statusStrip1->Text = L"statusStrip1";
			// 
			// toolStripStatusLabel1
			// 
			this->toolStripStatusLabel1->Name = L"toolStripStatusLabel1";
			this->toolStripStatusLabel1->Size = System::Drawing::Size(118, 17);
			this->toolStripStatusLabel1->Text = L"toolStripStatusLabel1";
			// 
			// toolStrip1
			// 
			this->toolStrip1->Items->AddRange(gcnew cli::array< System::Windows::Forms::ToolStripItem^  >(14) {this->StartB, this->OpenB, 
				this->Kiiro, this->toolStripSeparator1, this->jForg, this->bForg, this->Zoomin, this->Zoomout, this->toolStripButton2, this->toolStripButton3, 
				this->toolStripButton1, this->toolStripLabel1, this->toolStripSeparator2, this->ger_vi});
			this->toolStrip1->Location = System::Drawing::Point(0, 0);
			this->toolStrip1->Name = L"toolStrip1";
			this->toolStrip1->Size = System::Drawing::Size(641, 25);
			this->toolStrip1->TabIndex = 3;
			this->toolStrip1->Text = L"toolStrip1";
			this->toolStrip1->ItemClicked += gcnew System::Windows::Forms::ToolStripItemClickedEventHandler(this, &Form1::toolStrip1_ItemClicked);
			// 
			// StartB
			// 
			this->StartB->Image = (cli::safe_cast<System::Drawing::Image^  >(resources->GetObject(L"StartB.Image")));
			this->StartB->ImageTransparentColor = System::Drawing::Color::Olive;
			this->StartB->Name = L"StartB";
			this->StartB->Size = System::Drawing::Size(51, 22);
			this->StartB->Text = L"indít";
			this->StartB->Click += gcnew System::EventHandler(this, &Form1::StartB_Click);
			// 
			// OpenB
			// 
			this->OpenB->Image = (cli::safe_cast<System::Drawing::Image^  >(resources->GetObject(L"OpenB.Image")));
			this->OpenB->ImageTransparentColor = System::Drawing::Color::Purple;
			this->OpenB->Name = L"OpenB";
			this->OpenB->Size = System::Drawing::Size(71, 22);
			this->OpenB->Text = L"megnyit";
			this->OpenB->Click += gcnew System::EventHandler(this, &Form1::OpenB_Click);
			// 
			// Kiiro
			// 
			this->Kiiro->Name = L"Kiiro";
			this->Kiiro->Size = System::Drawing::Size(16, 22);
			this->Kiiro->Text = L"   ";
			// 
			// toolStripSeparator1
			// 
			this->toolStripSeparator1->Name = L"toolStripSeparator1";
			this->toolStripSeparator1->Size = System::Drawing::Size(6, 25);
			// 
			// jForg
			// 
			this->jForg->DisplayStyle = System::Windows::Forms::ToolStripItemDisplayStyle::Image;
			this->jForg->Image = (cli::safe_cast<System::Drawing::Image^  >(resources->GetObject(L"jForg.Image")));
			this->jForg->ImageTransparentColor = System::Drawing::Color::Olive;
			this->jForg->Name = L"jForg";
			this->jForg->Size = System::Drawing::Size(23, 22);
			this->jForg->Text = L"jobbra forgat";
			this->jForg->Click += gcnew System::EventHandler(this, &Form1::jForg_Click);
			// 
			// bForg
			// 
			this->bForg->DisplayStyle = System::Windows::Forms::ToolStripItemDisplayStyle::Image;
			this->bForg->Image = (cli::safe_cast<System::Drawing::Image^  >(resources->GetObject(L"bForg.Image")));
			this->bForg->ImageTransparentColor = System::Drawing::Color::Olive;
			this->bForg->Name = L"bForg";
			this->bForg->Size = System::Drawing::Size(23, 22);
			this->bForg->Text = L"balra forgat";
			this->bForg->Click += gcnew System::EventHandler(this, &Form1::bForg_Click);
			// 
			// Zoomin
			// 
			this->Zoomin->DisplayStyle = System::Windows::Forms::ToolStripItemDisplayStyle::Image;
			this->Zoomin->Image = (cli::safe_cast<System::Drawing::Image^  >(resources->GetObject(L"Zoomin.Image")));
			this->Zoomin->ImageTransparentColor = System::Drawing::Color::Olive;
			this->Zoomin->Name = L"Zoomin";
			this->Zoomin->Size = System::Drawing::Size(23, 22);
			this->Zoomin->Text = L"nagyít";
			this->Zoomin->Click += gcnew System::EventHandler(this, &Form1::Zoomin_Click);
			// 
			// Zoomout
			// 
			this->Zoomout->DisplayStyle = System::Windows::Forms::ToolStripItemDisplayStyle::Image;
			this->Zoomout->Image = (cli::safe_cast<System::Drawing::Image^  >(resources->GetObject(L"Zoomout.Image")));
			this->Zoomout->ImageTransparentColor = System::Drawing::Color::Olive;
			this->Zoomout->Name = L"Zoomout";
			this->Zoomout->Size = System::Drawing::Size(23, 22);
			this->Zoomout->Text = L"kicsinyít";
			this->Zoomout->Click += gcnew System::EventHandler(this, &Form1::Zoomout_Click);
			// 
			// toolStripButton2
			// 
			this->toolStripButton2->DisplayStyle = System::Windows::Forms::ToolStripItemDisplayStyle::Image;
			this->toolStripButton2->Image = (cli::safe_cast<System::Drawing::Image^  >(resources->GetObject(L"toolStripButton2.Image")));
			this->toolStripButton2->ImageTransparentColor = System::Drawing::Color::Olive;
			this->toolStripButton2->Name = L"toolStripButton2";
			this->toolStripButton2->Size = System::Drawing::Size(23, 22);
			this->toolStripButton2->Text = L"fel";
			this->toolStripButton2->Click += gcnew System::EventHandler(this, &Form1::toolStripButton2_Click);
			// 
			// toolStripButton3
			// 
			this->toolStripButton3->DisplayStyle = System::Windows::Forms::ToolStripItemDisplayStyle::Image;
			this->toolStripButton3->Image = (cli::safe_cast<System::Drawing::Image^  >(resources->GetObject(L"toolStripButton3.Image")));
			this->toolStripButton3->ImageTransparentColor = System::Drawing::Color::Olive;
			this->toolStripButton3->Name = L"toolStripButton3";
			this->toolStripButton3->Size = System::Drawing::Size(23, 22);
			this->toolStripButton3->Text = L"alap";
			this->toolStripButton3->Click += gcnew System::EventHandler(this, &Form1::toolStripButton3_Click);
			// 
			// toolStripButton1
			// 
			this->toolStripButton1->DisplayStyle = System::Windows::Forms::ToolStripItemDisplayStyle::Image;
			this->toolStripButton1->Image = (cli::safe_cast<System::Drawing::Image^  >(resources->GetObject(L"toolStripButton1.Image")));
			this->toolStripButton1->ImageTransparentColor = System::Drawing::Color::Olive;
			this->toolStripButton1->Name = L"toolStripButton1";
			this->toolStripButton1->Size = System::Drawing::Size(23, 22);
			this->toolStripButton1->Text = L"le";
			this->toolStripButton1->Click += gcnew System::EventHandler(this, &Form1::toolStripButton1_Click);
			// 
			// toolStripLabel1
			// 
			this->toolStripLabel1->Name = L"toolStripLabel1";
			this->toolStripLabel1->Size = System::Drawing::Size(16, 22);
			this->toolStripLabel1->Text = L"   ";
			// 
			// toolStripSeparator2
			// 
			this->toolStripSeparator2->Name = L"toolStripSeparator2";
			this->toolStripSeparator2->Size = System::Drawing::Size(6, 25);
			// 
			// ger_vi
			// 
			this->ger_vi->CheckOnClick = true;
			this->ger_vi->DisplayStyle = System::Windows::Forms::ToolStripItemDisplayStyle::None;
			this->ger_vi->Image = (cli::safe_cast<System::Drawing::Image^  >(resources->GetObject(L"ger_vi.Image")));
			this->ger_vi->ImageTransparentColor = System::Drawing::Color::Olive;
			this->ger_vi->Name = L"ger_vi";
			this->ger_vi->Size = System::Drawing::Size(23, 22);
			this->ger_vi->Click += gcnew System::EventHandler(this, &Form1::ger_vi_Click);
			// 
			// openFileDialog1
			// 
			this->openFileDialog1->FileName = L"openFileDialog1";
			this->openFileDialog1->FileOk += gcnew System::ComponentModel::CancelEventHandler(this, &Form1::openFileDialog1_FileOk);
			// 
			// panel_r
			// 
			this->panel_r->Controls->Add(this->dGVr);
			this->panel_r->Controls->Add(this->tabControl1);
			this->panel_r->Dock = System::Windows::Forms::DockStyle::Right;
			this->panel_r->Location = System::Drawing::Point(521, 25);
			this->panel_r->Margin = System::Windows::Forms::Padding(2);
			this->panel_r->Name = L"panel_r";
			this->panel_r->Size = System::Drawing::Size(120, 219);
			this->panel_r->TabIndex = 4;
			this->panel_r->Resize += gcnew System::EventHandler(this, &Form1::panel_r_Resize);
			// 
			// dGVr
			// 
			this->dGVr->AllowUserToAddRows = false;
			this->dGVr->AllowUserToDeleteRows = false;
			this->dGVr->AllowUserToResizeColumns = false;
			this->dGVr->AllowUserToResizeRows = false;
			this->dGVr->ColumnHeadersHeightSizeMode = System::Windows::Forms::DataGridViewColumnHeadersHeightSizeMode::AutoSize;
			this->dGVr->Columns->AddRange(gcnew cli::array< System::Windows::Forms::DataGridViewColumn^  >(1) {this->Column1});
			this->dGVr->Location = System::Drawing::Point(101, 3);
			this->dGVr->Name = L"dGVr";
			this->dGVr->ReadOnly = true;
			this->dGVr->RowHeadersVisible = false;
			this->dGVr->Size = System::Drawing::Size(19, 219);
			this->dGVr->TabIndex = 1;
			// 
			// tabControl1
			// 
			this->tabControl1->Controls->Add(this->tabPage1);
			this->tabControl1->Controls->Add(this->tabPage2);
			this->tabControl1->Dock = System::Windows::Forms::DockStyle::Left;
			this->tabControl1->Location = System::Drawing::Point(0, 0);
			this->tabControl1->Name = L"tabControl1";
			this->tabControl1->SelectedIndex = 0;
			this->tabControl1->Size = System::Drawing::Size(94, 219);
			this->tabControl1->TabIndex = 0;
			// 
			// tabPage1
			// 
			this->tabPage1->Controls->Add(this->dGVgo);
			this->tabPage1->Location = System::Drawing::Point(4, 22);
			this->tabPage1->Name = L"tabPage1";
			this->tabPage1->Padding = System::Windows::Forms::Padding(3);
			this->tabPage1->Size = System::Drawing::Size(86, 193);
			this->tabPage1->TabIndex = 0;
			this->tabPage1->Text = L"Oldalnézet";
			this->tabPage1->UseVisualStyleBackColor = true;
			this->tabPage1->Paint += gcnew System::Windows::Forms::PaintEventHandler(this, &Form1::tabPage1_Paint);
			this->tabPage1->Resize += gcnew System::EventHandler(this, &Form1::tabPage1_Resize);
			// 
			// dGVgo
			// 
			this->dGVgo->AllowUserToAddRows = false;
			this->dGVgo->AllowUserToDeleteRows = false;
			this->dGVgo->AllowUserToResizeColumns = false;
			this->dGVgo->AllowUserToResizeRows = false;
			this->dGVgo->ColumnHeadersHeightSizeMode = System::Windows::Forms::DataGridViewColumnHeadersHeightSizeMode::AutoSize;
			this->dGVgo->Columns->AddRange(gcnew cli::array< System::Windows::Forms::DataGridViewColumn^  >(2) {this->Cso, this->Go});
			this->dGVgo->Dock = System::Windows::Forms::DockStyle::Right;
			this->dGVgo->Location = System::Drawing::Point(45, 3);
			this->dGVgo->Name = L"dGVgo";
			this->dGVgo->ReadOnly = true;
			this->dGVgo->RowHeadersVisible = false;
			this->dGVgo->Size = System::Drawing::Size(38, 187);
			this->dGVgo->TabIndex = 0;
			// 
			// tabPage2
			// 
			this->tabPage2->Controls->Add(this->dGVgh);
			this->tabPage2->Location = System::Drawing::Point(4, 22);
			this->tabPage2->Name = L"tabPage2";
			this->tabPage2->Padding = System::Windows::Forms::Padding(3);
			this->tabPage2->Size = System::Drawing::Size(86, 193);
			this->tabPage2->TabIndex = 1;
			this->tabPage2->Text = L"Hátulnézet";
			this->tabPage2->UseVisualStyleBackColor = true;
			this->tabPage2->Paint += gcnew System::Windows::Forms::PaintEventHandler(this, &Form1::tabPage2_Paint);
			this->tabPage2->Resize += gcnew System::EventHandler(this, &Form1::tabPage2_Resize);
			// 
			// dGVgh
			// 
			this->dGVgh->ColumnHeadersHeightSizeMode = System::Windows::Forms::DataGridViewColumnHeadersHeightSizeMode::AutoSize;
			this->dGVgh->Columns->AddRange(gcnew cli::array< System::Windows::Forms::DataGridViewColumn^  >(2) {this->dataGridViewTextBoxColumn1, 
				this->dataGridViewTextBoxColumn2});
			this->dGVgh->Dock = System::Windows::Forms::DockStyle::Right;
			this->dGVgh->Location = System::Drawing::Point(45, 3);
			this->dGVgh->Name = L"dGVgh";
			this->dGVgh->RowHeadersVisible = false;
			this->dGVgh->SelectionMode = System::Windows::Forms::DataGridViewSelectionMode::FullRowSelect;
			this->dGVgh->Size = System::Drawing::Size(38, 187);
			this->dGVgh->TabIndex = 1;
			// 
			// dataGridViewTextBoxColumn1
			// 
			this->dataGridViewTextBoxColumn1->HeaderText = L"Cs";
			this->dataGridViewTextBoxColumn1->Name = L"dataGridViewTextBoxColumn1";
			// 
			// dataGridViewTextBoxColumn2
			// 
			this->dataGridViewTextBoxColumn2->HeaderText = L"G";
			this->dataGridViewTextBoxColumn2->Name = L"dataGridViewTextBoxColumn2";
			// 
			// panel_l
			// 
			this->panel_l->Controls->Add(this->dgxyz);
			this->panel_l->Dock = System::Windows::Forms::DockStyle::Left;
			this->panel_l->Location = System::Drawing::Point(0, 25);
			this->panel_l->Margin = System::Windows::Forms::Padding(2);
			this->panel_l->Name = L"panel_l";
			this->panel_l->Size = System::Drawing::Size(115, 219);
			this->panel_l->TabIndex = 5;
			// 
			// dgxyz
			// 
			this->dgxyz->AllowUserToAddRows = false;
			this->dgxyz->AllowUserToDeleteRows = false;
			this->dgxyz->AllowUserToResizeColumns = false;
			this->dgxyz->AllowUserToResizeRows = false;
			this->dgxyz->ColumnHeadersHeightSizeMode = System::Windows::Forms::DataGridViewColumnHeadersHeightSizeMode::AutoSize;
			this->dgxyz->Columns->AddRange(gcnew cli::array< System::Windows::Forms::DataGridViewColumn^  >(3) {this->x_c, this->y_c, 
				this->z_c});
			this->dgxyz->Location = System::Drawing::Point(2, 24);
			this->dgxyz->Margin = System::Windows::Forms::Padding(2);
			this->dgxyz->Name = L"dgxyz";
			this->dgxyz->ReadOnly = true;
			this->dgxyz->RowHeadersVisible = false;
			this->dgxyz->RowTemplate->Height = 24;
			this->dgxyz->RowTemplate->Resizable = System::Windows::Forms::DataGridViewTriState::False;
			this->dgxyz->SelectionMode = System::Windows::Forms::DataGridViewSelectionMode::FullRowSelect;
			this->dgxyz->Size = System::Drawing::Size(100, 67);
			this->dgxyz->TabIndex = 6;
			this->dgxyz->SelectionChanged += gcnew System::EventHandler(this, &Form1::dgxyz_SelectionChanged);
			// 
			// x_c
			// 
			this->x_c->HeaderText = L"x";
			this->x_c->Name = L"x_c";
			this->x_c->ReadOnly = true;
			this->x_c->Width = 120;
			// 
			// y_c
			// 
			this->y_c->HeaderText = L"y";
			this->y_c->Name = L"y_c";
			this->y_c->ReadOnly = true;
			// 
			// z_c
			// 
			this->z_c->HeaderText = L"z";
			this->z_c->Name = L"z_c";
			this->z_c->ReadOnly = true;
			// 
			// G
			// 
			this->G->HeaderText = L"G";
			this->G->Name = L"G";
			// 
			// Cs
			// 
			this->Cs->HeaderText = L"Cs";
			this->Cs->Name = L"Cs";
			// 
			// Column1
			// 
			this->Column1->HeaderText = L"Torzió";
			this->Column1->Name = L"Column1";
			this->Column1->ReadOnly = true;
			// 
			// Cso
			// 
			this->Cso->HeaderText = L"Csigolya";
			this->Cso->Name = L"Cso";
			this->Cso->ReadOnly = true;
			// 
			// Go
			// 
			this->Go->HeaderText = L"Görbület";
			this->Go->Name = L"Go";
			this->Go->ReadOnly = true;
			// 
			// Form1
			// 
			this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
			this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
			this->ClientSize = System::Drawing::Size(641, 266);
			this->Controls->Add(this->panel_l);
			this->Controls->Add(this->panel_r);
			this->Controls->Add(this->toolStrip1);
			this->Controls->Add(this->statusStrip1);
			this->Name = L"Form1";
			this->Text = L"Form1";
			this->WindowState = System::Windows::Forms::FormWindowState::Maximized;
			this->Load += gcnew System::EventHandler(this, &Form1::Form1_Load);
			this->MouseUp += gcnew System::Windows::Forms::MouseEventHandler(this, &Form1::Form1_MouseUp);
			this->Shown += gcnew System::EventHandler(this, &Form1::Form1_Shown);
			this->Resize += gcnew System::EventHandler(this, &Form1::Form1_Resize);
			this->MouseMove += gcnew System::Windows::Forms::MouseEventHandler(this, &Form1::Form1_MouseMove);
			this->statusStrip1->ResumeLayout(false);
			this->statusStrip1->PerformLayout();
			this->toolStrip1->ResumeLayout(false);
			this->toolStrip1->PerformLayout();
			this->panel_r->ResumeLayout(false);
			(cli::safe_cast<System::ComponentModel::ISupportInitialize^  >(this->dGVr))->EndInit();
			this->tabControl1->ResumeLayout(false);
			this->tabPage1->ResumeLayout(false);
			(cli::safe_cast<System::ComponentModel::ISupportInitialize^  >(this->dGVgo))->EndInit();
			this->tabPage2->ResumeLayout(false);
			(cli::safe_cast<System::ComponentModel::ISupportInitialize^  >(this->dGVgh))->EndInit();
			this->panel_l->ResumeLayout(false);
			(cli::safe_cast<System::ComponentModel::ISupportInitialize^  >(this->dgxyz))->EndInit();
			this->ResumeLayout(false);
			this->PerformLayout();

		}
#pragma endregion

		cli::array<String^>^ portnevek;
		String^ s;
		static bool running = false;
		bool button,kiiras;
        double xn, yn, zn, xh, yh, zh, mozdul, hossz, hossz_kalib;
        int cx, cy, cz,xo,yo,zo,cnt,cnt_e,merszam,ido;
		int *answ,*x,*y,*z;

void get_coords()
        {
           int i,x_min,x_max,y_min,y_max,z_min,z_max, xs, ys, zs;
		   double xd, yd, zd,hossz;
		   xs=ys=zs=0;
            serialPort1->Write("9"); // get coords
            for (i = 0; i < 63; i++) answ[i] = serialPort1->ReadByte();
            for (i = 0; i < 10; i++)
            {
				if (global::danka)
				{
					x[i] = (answ[6*i] + 256 * answ[6*i+1]) - 0x200;
					y[i] = (answ[6*i+2] + 256 * answ[6*i+3]) - 0x200;
					z[i] = (answ[6*i+4] + 256 * answ[6*i+5]) - 0x200;
				}
				else
				{
	                x[i] = (answ[6*i] + 256 * answ[6*i+1]) - 0x200;
		            y[i] = (answ[6*i+2] + 256 * answ[6*i+3]) - 0x200;
			        z[i] = (answ[6*i+4] + 256 * answ[6*i+5]) - 0x200;
				}
				xs += x[i]; ys += y[i]; zs += z[i];
				if (i==0)
				{
					x_min=x_max=x[i];
					y_min=y_max=y[i];
					z_min=z_max=z[i];
				} else 
				{
					if (x[i]<x_min) x_min=x[i];
					if (x[i]>x_max) x_max=x[i];
					if (y[i]<y_min) y_min=y[i];
					if (y[i]>y_max) y_max=y[i];
					if (z[i]<z_min) z_min=z[i];
					if (z[i]>z_max) z_max=z[i];
				}
            }
			xs=xs-(x_min+x_max);
			ys=ys-(y_min+y_max);
			zs=zs-(z_min+z_max);
            xd = (double)xs / 8; yd = (double)ys / 8; zd = (double)zs / 8;
			// calibration only
//			xo = (int)((double)xs / 8+0.5); 
//          yo = (int)((double)ys / 8+0.5); 
//          zo = (int)((double)zs / 8+0.5);
			cnt=(256*answ[60]+answ[61]);
			if (cnt > 32767) cnt -= 65536;
			hossz = Math::Sqrt(xd * xd + yd * yd + zd * zd);
            xn = xd / hossz; // normal vector. (c) TP
            yn = yd / hossz;
            zn = zd / hossz;
			button=!answ[62];
			if (kiiras)
            toolStripStatusLabel1->Text = "db="+point_db.ToString()+"."+
				"  xh="+xh.ToString("0.000")+
				"  yh="+yh.ToString("0.000")+
				"  zh="+zh.ToString("0.000")+ "            "+
				" (xn=" + xn.ToString("0.0000") +
                " yn=" + yn.ToString("0.0000") +
                " zn=" + zn.ToString("0.0000") +
                " a=" + hossz.ToString("0.000")+")";
				//" cnt=" + Convert::ToString(cnt)+
				//" xh="+xh.ToString("0.0")+
				//" yh="+yh.ToString("0.0")+
				//" zh="+zh.ToString("0.0");
        }

	private: System::Void Form1_Load(System::Object^  sender, System::EventArgs^  e) 
			 {
				global::points=new g3d[10000];
				global::kt=new p3d[10000];
				global::kb=new p3d[10000];
				global::kn=new p3d[10000];
				for (int i =0; i<10000; i++)
				{
					global::points[i].ivh=0;
					global::points[i].xr=0;
					global::points[i].yr=0;
					global::points[i].zr=0;
					global::kt[i].x=0;
					global::kt[i].y=0;
					global::kt[i].z=0;
					global::kb[i].x=0;
					global::kb[i].y=0;
					global::kb[i].z=0;
					global::kn[i].x=0;
					global::kn[i].y=0;
					global::kn[i].z=0;
				}



				bool van = false;
				int i = 0;
				portnevek = System::IO::Ports::SerialPort::GetPortNames();

				while ((!van) && (i < portnevek->Length))
			{
                serialPort1->PortName = portnevek[i];
                serialPort1->BaudRate = 9600;
                serialPort1->DataBits = 8;
				serialPort1->StopBits = System::IO::Ports::StopBits::One;
				serialPort1->Parity = System::IO::Ports::Parity::None;
				serialPort1->Handshake = System::IO::Ports::Handshake::None;
                serialPort1->ReadTimeout = 300; // 0.3 s
                serialPort1->NewLine = "\r\n";    
                try
                {  // bluetooth in Win7.
					serialPort1->Open();
					serialPort1->DiscardInBuffer();
					serialPort1->Write("?");
                    s = serialPort1->ReadLine();
                }
                catch (...)
                {
                    s = "Timeout";
                    serialPort1->Close();
                }
                if (s == "ge-usb")
                {
                    van = true;
                }
                else serialPort1->Close();
                i++;
            }
			if (van)
            {
                this->Text = "usb 3d ctrl on " +
                    serialPort1->PortName;
                toolStripStatusLabel1->Text = this->Text;
            }
            else
            {
				MessageBox::Show("No 3d ctrl", "Error",
					MessageBoxButtons::OK);
				//Application::Exit();
				StartB->Enabled=false;
            }

			FILE * pfile=NULL;
			try
			{
   			    int hg=0;
				pfile=fopen("C:\\scan\\Hatgerinc.txt","r");
				if (pfile)
				{
					//fscanf(pfile,"%i",hg);
					char chg[256];
					fgets(chg,255,pfile);
					hg=atoi(chg);
 					global::danka=hg;
					fclose(pfile);
				}
			}
			catch (...)
			{
 				global::danka=false;
			}
			if (global::danka)
				hossz_kalib = (double)315/712; // 712 cnt = 315mm ???
			else
				hossz_kalib = (double)500/1031; // 712 cnt = 315mm ???
           
            answ=new int[64];// full puffer
			x = new int[10];
			y = new int[10];
    		z = new int[10];
			global::iv_alap=alap_ivhossz();
			ido=0;
	//		timer1->Interval=1000; //1s
	//		timer1->Enabled=true;

 }





private: System::Void timer1_Tick(System::Object^  sender, System::EventArgs^  e) {

			if (ido) 
				toolStripStatusLabel1->Text="mps:"+Convert::ToString((double)merszam/ido);
			ido++;
		 }
private: System::Void Form1_Shown(System::Object^  sender, System::EventArgs^  e) {
//				hwnd=(HWND)this->pictureBox1->Handle.ToInt32();
				hwnd=(HWND)this->Handle.ToInt32();
				m_hDC = GetDC(hwnd);
				if(m_hDC) {
					wglMakeCurrent(m_hDC, NULL); 
					MySetPixelFormat(m_hDC);
					InitGL();
//					rsize(pictureBox1->Width, pictureBox1->Height, 1, 200);
					rsize(this->ClientRectangle.Width, this->ClientRectangle.Height, 1, hatul);
					Render();
					SwapBuffers(m_hDC);
				}
		 }

		System::Void Render(System::Void)
		{
			glLineWidth((GLfloat)1);
			glPushMatrix();
			glTranslated(0.0, 0.0, -tav);
			glRotatef(nezszog, 1.0, 0.0, 0.0);
			glTranslated(0.0, 0.0, -szemmag/2.5);
			glRotatef(testszog, 0.0, 0.0, 1.0);
			glScalef(0.5,0.5,0.5);
			glClearColor(0.5,0.5,0.5,1);
  			glClear( GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT );
			glPolygonMode(GL_FRONT_AND_BACK , GL_FILL);
			glDisable(GL_TEXTURE_2D);
			glDisable(GL_LIGHTING);
			glDisable(GL_LIGHT0);

			glColor3d(1,0,0);
			glBegin(GL_LINES);
				glVertex3d(0,0,0);
				glVertex3d(100,0,0);
			glEnd();
			glColor3d(0,1,0);
			glBegin(GL_LINES);
				glVertex3d(0,0,0);
				glVertex3d(0,100,0);
			glEnd();
			glColor3d(0,0,1);
			glBegin(GL_LINES);
				glVertex3d(0,0,0);
				glVertex3d(0,0,100);
			glEnd();
			int kh=30;
			for (int i=0; i<point_db ; i++)
			{
				Kocka(global::points[i].xr,
					  global::points[i].yr,
					  global::points[i].zr,10,0);
			}
			glLineWidth((GLfloat)2);
			int i=0;
			if ((kiv>-1)&&(point_db>0))
			{
				int i=kiv;
				Kocka(global::points[i].xr,
					  global::points[i].yr,
					  global::points[i].zr,10,1);
				glColor3d(0,0,1);
				glBegin(GL_LINES);
						glVertex3d(global::points[i].xr,global::points[i].yr,global::points[i].zr);
						glVertex3d(global::points[i].xr+kh*global::kt[i].x,
							       global::points[i].yr+kh*global::kt[i].y,
								   global::points[i].zr+kh*global::kt[i].z);
				glEnd();
				glColor3d(0,1,0);
						glBegin(GL_LINES);
						glVertex3d(global::points[i].xr,global::points[i].yr,global::points[i].zr);
						glVertex3d(global::points[i].xr+kh*global::kb[i].x,
							       global::points[i].yr+kh*global::kb[i].y,
								   global::points[i].zr+kh*global::kb[i].z);
				glEnd();

				glColor3d(1,0,0);
				glBegin(GL_LINES);
						glVertex3d(global::points[i].xr,global::points[i].yr,global::points[i].zr);
						glVertex3d(global::points[i].xr+kh*global::kn[i].x,
							       global::points[i].yr+kh*global::kn[i].y,
								   global::points[i].zr+kh*global::kn[i].z);
				glEnd();
			}
			Kocka(0,0,0,10.00,0);
			//const GLfloat test_szort[]  ={0.8,0.68,0.58,1.0};
			//const GLfloat test_tukros[] ={0.3,0.18,0.08,1.0};
			//const GLfloat	test_fenyes[]= {30.0};

        const GLfloat test_szort[]={ 0.698, 0.824, 0.824, 1, }; // Ambient color
        const GLfloat test_tukros[] ={ 0.698, 0.824, 0.824, 1, }; // Diffuse color
        const GLfloat test_fenyes[]= //{ 0.698, 0.824, 0.824, 1, }, // Specular color
        { 0.5}; // Emissive color

			GLfloat light_position[] = { 1.0, -5.0, 1.0, 0.0 };
			glColorMaterial(GL_FRONT,GL_AMBIENT_AND_DIFFUSE);
			glEnable(GL_COLOR_MATERIAL);
			glEnable(GL_LIGHTING);
			glEnable(GL_LIGHT0);
			glShadeModel (GL_SMOOTH);
			glEnable(GL_DEPTH_TEST);
			glMaterialfv(GL_FRONT,GL_DIFFUSE,test_szort);     // szórt fény
			glMaterialfv(GL_FRONT,GL_SPECULAR,test_tukros);   // tükrözõdés
		    glMaterialfv(GL_FRONT,GL_SHININESS,test_fenyes);  // fényesség
			if (global::vangerinc&&(point_db>0))
			   gerincki();

			glPopMatrix();
		}

		void  rsize (int szelesseg, int magassag, int elol,  int hatul) {
			glViewport(0, 0, szelesseg, magassag);
			float viszony = float(szelesseg) / magassag;
			glMatrixMode( GL_PROJECTION );
			glLoadIdentity();
			gluPerspective( zoomszog, viszony, elol, hatul );
			glMatrixMode( GL_MODELVIEW );
			tav = float (elol+hatul)/2;
			}

private: System::Void StartB_Click(System::Object^  sender, System::EventArgs^  e) {
			NameIn ^ kerdes = gcnew NameIn();			              


			if ( kerdes->ShowDialog() == Windows::Forms::DialogResult::OK)
			{
				global::vangerinc=false;
				file_name=NameIn::pac_name;
				if (file_name->Length==0)
					return;
				if (!filename_ok(file_name)) {
					System::Windows::Forms::MessageBox::Show("Hibás filenév!\n"+
							"0..9,a..z,A..Z","Error",MessageBoxButtons::OK);
					return;
				}
                datedir=gcnew String("");
				datumido=gcnew DateTime();
                datumido=datumido->Now;


                datedir+=datumido->Year.ToString("0000")+datumido->Month.ToString("00")+
                datumido->Day.ToString("00");
                if (! Directory::Exists(outputdir+datedir)) { // no directory yet
                    Directory::CreateDirectory(outputdir+datedir);  // we are making it
                }
				Kiiro->Text = "Az egéren lévõ gomb megnyomása indítja a mérést";
				do
				{
	                kiiras = true;
		            get_coords();
					Application::DoEvents();
	            } while (!button);
		        kiiras = true;


			    Kiiro->Text = "Az egéren lévõ gomb megnyomása megállítja a mérést";
				Application::DoEvents();
				db = 0;
				serialPort1->Write("0"); // zero cnt
	            s = serialPort1->ReadLine();
				cnt_e = 0;
				merszam=0; ido=0;
				point_db = 0;
				global::points[point_db].ivh = 0;
				dgxyz->Rows->Clear();
				kiv=0;
				xh = yh = zh = 0;
				Form1_Resize(sender, e);
				do
				{
					get_coords();
					merszam++;
			        if (cnt != cnt_e) // volt kerék forgás ?
					{
					//	toolStripStatusLabel1->Text=Convert::ToString(cnt-cnt_e);
						mozdul = hossz_kalib * (cnt - cnt_e);
						//xh += mozdul * zn;
						//yh += mozdul * yn;
						//zh += mozdul * xn;
						if (point_db>0)
						   global::points[point_db].ivh = global::points[point_db-1].ivh+mozdul;
						
						if (global::danka)
						{
							xh -= mozdul * zn;
							yh += mozdul * xn;
							zh -= mozdul * yn;
						}
						else
						{
							xh -= mozdul * zn;
							yh += mozdul * yn;
							zh += mozdul * xn;
						}

						try{
							global::points[point_db].xr = xh;
							global::points[point_db].yr = yh;
							global::points[point_db].zr = zh;
						}
						catch(...)
						{
						}

						sx=String::Format("{0, 8:f2}", xh);
						sy=String::Format("{0, 8:f2}", yh);
						sz=String::Format("{0, 8:f2}", zh);
						dgxyz->Rows->Add( sx, sy,sz);
						point_db++;
						Render();
						SwapBuffers(m_hDC);
						cnt_e = cnt;
		            }
					Application::DoEvents();
	            } while (!button || (point_db < 2));
		        Kiiro->Text = "Kész";
				try {
					fw0=gcnew StreamWriter(outputdir+datedir+"\\"+file_name+"-"+datum2string(datumido)+".txt");
					for (int i=0; i<point_db; i++)
						fw0->WriteLine("{0,12:f2} ; {1,12:f2} ; {2,12:f2}",global::points[i].xr,global::points[i].yr,global::points[i].zr);
					fw0->Flush();
					fw0->Close();
				}
				catch (...)
				{
					MessageBox::Show("Sikertelen mentés", "Error",
						MessageBoxButtons::OK);
				}

			}
			for (int i=0; i<point_db; i++)
			{
				d1(i);
			}
			for (int i=0; i<point_db; i++)
			{
				d2(i);
			}
			for (int i=0; i<point_db; i++)
			{
				ktr(i);
			}



		 }
private: System::Void label2_Click(System::Object^  sender, System::EventArgs^  e) {
		 }
private: System::Void jForg_Click(System::Object^  sender, System::EventArgs^  e) {
			testszog+=10;
   			Render();
			SwapBuffers(m_hDC);
		 }
private: System::Void bForg_Click(System::Object^  sender, System::EventArgs^  e) {
			testszog-=10;
   			Render();
			SwapBuffers(m_hDC);
		 }
private: System::Void Zoomin_Click(System::Object^  sender, System::EventArgs^  e) {
			 if (zoomszog>15)
			 {
				zoomszog-=5;
				rsize(this->ClientRectangle.Width, this->ClientRectangle.Height, 1, hatul);
				Render();
				SwapBuffers(m_hDC);
			}
		 }
private: System::Void Zoomout_Click(System::Object^  sender, System::EventArgs^  e) {
			 if (zoomszog<150)
			 {
				zoomszog+=5;
				rsize(this->ClientRectangle.Width, this->ClientRectangle.Height, 1, hatul);
				Render();
				SwapBuffers(m_hDC);
			 }
		 }
private: System::Void toolStrip1_ItemClicked(System::Object^  sender, System::Windows::Forms::ToolStripItemClickedEventArgs^  e) {
		 }
private: System::Void toolStripButton2_Click(System::Object^  sender, System::EventArgs^  e) {
				if (szemmag>-550)
				{
					szemmag-=10;
					rsize(this->ClientRectangle.Width, this->ClientRectangle.Height, 1, hatul);
					Render();
					SwapBuffers(m_hDC);
				}
		 }
private: System::Void toolStripButton1_Click(System::Object^  sender, System::EventArgs^  e) {
				if (szemmag<500)
				{
					szemmag+=10;
					rsize(this->ClientRectangle.Width, this->ClientRectangle.Height, 1, hatul);
					Render();
					SwapBuffers(m_hDC);
				}
		 }
private: System::Void toolStripButton3_Click(System::Object^  sender, System::EventArgs^  e) {

			zoomszog=100;
			nezszog=-90.0;
			hatul=400.0;
			szemmag=-350;
			testszog=0;//-45;
			rsize(this->ClientRectangle.Width, this->ClientRectangle.Height, 1, hatul);
			Render();
			SwapBuffers(m_hDC);
		 }
private: System::Void Form1_Resize(System::Object^  sender, System::EventArgs^  e) {
			panel_l->Width=this->ClientRectangle.Width/4;
			for (int i=0; i<3; i++)
				dgxyz->Columns[i]->Width=(panel_l->Width-4)/3;
			dgxyz->Top=0;
			dgxyz->Left=0;
			dgxyz->Width=panel_l->Width;
			dgxyz->Height=panel_l->Height;
   			panel_r->Width=this->ClientRectangle.Width/4;
			rsize(this->ClientRectangle.Width, this->ClientRectangle.Height, 1, hatul);
			Render();
			SwapBuffers(m_hDC);
		 }
private: System::Void OpenB_Click(System::Object^  sender, System::EventArgs^  e) {
	openFileDialog1->InitialDirectory = "c:\\scan\\"; 
	openFileDialog1->Filter = "txt files (*.txt)|*.txt"; 
	if ( openFileDialog1->ShowDialog() == System::Windows::Forms::DialogResult::OK ) {
		global::vangerinc=true;
	    ger_vi->DisplayStyle=ToolStripItemDisplayStyle::Image;
		try {
			StreamReader ^ sr = File::OpenText( openFileDialog1->FileName); 
			String ^ s = ""; 
			String ^ ss1;
			point_db=0;
			global::points[point_db].ivh=0;
			dgxyz->Rows->Clear();
			kiv=0;
			String ^ ss= CultureInfo::CurrentCulture->Name; 
			while ( !sr->EndOfStream) { 
				s = sr->ReadLine();
				String ^ elv=";";
				int i=0;
				while (s != "") {
					ss1=ParseCoord(s, elv)->Trim();
					if (ss->IndexOf("HU")>-1)
						ss1=ss1->Replace('.',',');
					else
						ss1=ss1->Replace(',','.');
					switch (i){
						case 0:
							global::points[point_db].xr=0;
							global::points[point_db].xr=Convert::ToDouble(ss1);
							break;
						case 1:
							global::points[point_db].yr=Convert::ToDouble(ss1);
							break;
						case 2:
							global::points[point_db].zr=Convert::ToDouble(ss1);
							break;
					}
					i++;
				}
				sx=String::Format("{0, 8:f2}", global::points[point_db].xr);
				sy=String::Format("{0, 8:f2}", global::points[point_db].yr);
				sz=String::Format("{0, 8:f2}", global::points[point_db].zr);
				dgxyz->Rows->Add( sx, sy, sz);
				Application::DoEvents();
				if (point_db>0)
				  global::points[point_db].ivh=global::points[point_db-1].ivh+
							sqrt(pow(global::points[point_db].xr-global::points[point_db-1].xr,2)+
								 pow(global::points[point_db].yr-global::points[point_db-1].yr,2)+
							     pow(global::points[point_db].zr-global::points[point_db-1].zr,2));
				if (point_db==0)
					global::mert_iv[point_db]=0;
				else 
					global::mert_iv[point_db]=global::points[point_db-1].ivh;
				global::mert_x[point_db]=global::points[point_db].xr;
				global::mert_y[point_db]=global::points[point_db].yr;
				global::mert_z[point_db]=global::points[point_db].zr;
				// this->Text=Convert::ToString(points[point_db].ivh);  
				sx=sx;
				global::mertp=point_db;
				point_db++;
			}
			sr->Close();



			global::iv_mert=global::mert_iv[point_db-1];
			for (int i=0; i<point_db; i++)
			{
				d1(i);
			}
			for (int i=0; i<point_db; i++)
			{
				d2(i);
			}
			for (int i=0; i<point_db; i++)
			{
				ktr(i);
			}
			double x=global::kt[12].x;
			passz(global::mertp, point_db);
			double xx=global::kt[12].x;
			global::vangerinc=true;
			ger_vi->Checked=true;
			


			Form1_Resize(sender, e);
			tabPage1->Refresh();
			// kisero trieder


		} 
		catch (...)
		{
					MessageBox::Show("Sikertelen nyitás", "Error",
						MessageBoxButtons::OK);
		}
		//if (point_db>0)
		//for (int i=0; i<1; i++)
		//{
		//	sx=String::Format("{0, 8:f2}", points[i].xr);
		//	sy=String::Format("{0, 8:f2}", points[i].yr);
		//	sz=String::Format("{0, 8:f2}", points[i].zr);
		//	dgxyz->Rows->Add( sx, sy, sz);
		//}
	}
}
private: System::Void openFileDialog1_FileOk(System::Object^  sender, System::ComponentModel::CancelEventArgs^  e) {
		 }
private: System::Void Form1_MouseMove(System::Object^  sender, System::Windows::Forms::MouseEventArgs^  e) {
			 if (e->Button==System::Windows::Forms::MouseButtons::Left)
			 {
				if ((XE!=-1000)&&(YE!=-1000)) 
				{
					float hossz=vh2(e->X,e->Y,XE,YE);
					if (hossz>10)
					{
						float dx=(e->X-XE)/hossz;
						float dy=(e->Y-YE)/hossz;
						float xt=vh2(dx,dy,1,0);
						float mxt=vh2(dx,dy,-1,0);
						float yt=vh2(dx,dy,0,1);
						float myt=vh2(dx,dy,0,-1);
						float mt=min(xt,min(mxt,min(yt,myt)));
						if (mt==xt)
							jForg_Click(sender, (System::EventArgs^)  e);
						if (mt==mxt)
							bForg_Click(sender, (System::EventArgs^)  e);
						if (mt==yt)
							toolStripButton1_Click(sender, (System::EventArgs^)  e);
						if (mt==myt)
							toolStripButton2_Click(sender, (System::EventArgs^)  e);
						XE=e->X;
						YE=e->Y;
					}
				}
				else
				{
					 XE=e->X;
					 YE=e->Y;
				}
			 }
			 if (e->Button==System::Windows::Forms::MouseButtons::Right) 
			 {
				if ((XE!=-1000)&&(YE!=-1000)) 
				{
			 	    float hossz=vh2(e->X,e->Y,XE,YE);
					if (hossz>10)
					{
						if (e->Y-YE>0)
							Zoomout_Click(sender, (System::EventArgs^)  e);
						else
							Zoomin_Click(sender, (System::EventArgs^)  e);
						XE=e->X;
						YE=e->Y;
					}
				}
				else
				{
					 XE=e->X;
					 YE=e->Y;
				}
			 }

		 }
private: System::Void Form1_MouseUp(System::Object^  sender, System::Windows::Forms::MouseEventArgs^  e) {
					 XE=-1000;
					 YE=-1000;
		 }
private: System::Void dgxyz_SelectionChanged(System::Object^  sender, System::EventArgs^  e) {
			 for (int i=0; i<point_db; i++)
				 if (dgxyz->Rows[i]->Selected==true)
				 {
					 kiv=i;
					 break;
				 }
				Render();
				tabPage1->Refresh();
				tabPage2->Refresh();
				SwapBuffers(m_hDC);
//				 this->Text=kiv.ToString();
		 }
private: System::Void ger_vi_Click(System::Object^  sender, System::EventArgs^  e) {
			 if (ger_vi->Checked)
			 {
				 ger_vi->DisplayStyle=ToolStripItemDisplayStyle::None;
				 global::vangerinc=false;
			 }
			 else
			 {
				 ger_vi->DisplayStyle=ToolStripItemDisplayStyle::Image;
				 global::vangerinc=true;
			 }
			Form1_Resize(sender, e);

		 }
private: System::Void toolStripButton4_Click(System::Object^  sender, System::EventArgs^  e) {
		 	openFileDialog1->InitialDirectory = "c:\\scan\\"; 
		if (point_db>0)
		{
			double ssxx=0;
			double ssyy=0;
			StreamWriter ^ sw = File::CreateText( openFileDialog1->FileName+"_"); 
			for (int i=0; i<point_db; i++)
			{
				ssxx=global::points[i].xr;
				ssyy=global::points[i].yr;
				global::points[i].xr=ssyy;
				global::points[i].yr=ssxx;
				sw->WriteLine("{0,12:f2} ; {1,12:f2} ; {2,12:f2}", 
							global::points[i].xr, global::points[i].yr, global::points[i].zr); 
			}
			sw->Close();
		}


	}
 //}
private: System::Void tabPage3_Paint(System::Object^  sender, System::Windows::Forms::PaintEventArgs^  e) {
		 }
private: System::Void tabPage1_Paint(System::Object^  sender, System::Windows::Forms::PaintEventArgs^  e) {
			if (global::vangerinc&&(point_db>0))
			{
				gerinckiki(e->Graphics,tabPage1->Width, tabPage1->Height , point_db, 1, 0, dGVgo, dGVr);
			}
			if ((kiv>-1)&&(point_db>0))
			{
	 			gerinckiki(e->Graphics,tabPage1->Width, tabPage1->Height , point_db, 1, kiv, dGVgo, dGVr );
			}


		 }
private: System::Void tabPage2_Paint(System::Object^  sender, System::Windows::Forms::PaintEventArgs^  e) {
			if (global::vangerinc&&(point_db>0))
			{
	   			 gerinckiki(e->Graphics,tabPage1->Width, tabPage1->Height , point_db, 0, 0, dGVgh, dGVr);
			}
			if ((kiv>-1)&&(point_db>0))
			{
   			    gerinckiki(e->Graphics,tabPage1->Width, tabPage1->Height , point_db, 0, kiv, dGVgh, dGVr);
			}
		 }
private: System::Void tabPage1_Resize(System::Object^  sender, System::EventArgs^  e) {
		 }
private: System::Void tabPage2_Resize(System::Object^  sender, System::EventArgs^  e) {
		 }
private: System::Void panel_r_Resize(System::Object^  sender, System::EventArgs^  e) {
			 tabControl1->Width=3*panel_r->Width/4;
			 dGVgo->Width=tabControl1->Width/2;
			 dGVgh->Width=tabControl1->Width/2;
			 dGVgo->Columns[0]->Width=dGVgo->Width/2;
			 dGVgo->Columns[1]->Width=dGVgo->Width/2;
			 dGVgh->Columns[0]->Width=dGVgo->Width/2;
			 dGVgh->Columns[1]->Width=dGVgo->Width/2;
			 dGVr->Width=panel_r->Width/4;
			 dGVr->Left=panel_r->Width-panel_r->Width/4;
			 dGVr->Top=25;
			 dGVr->Height=panel_r->Height-30;



		 }
};
}

