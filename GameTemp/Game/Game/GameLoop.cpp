#include <windows.h>
#include <gl/Gl.h>
#include <gl/Glu.h>
#include <iostream> 
#include "glut.h"
using namespace std;

//int frame=0,time,timebase=0;
int screenWidth = 800;
int screenHeight = 600;
char pressed;

void Game()
{
	glClear(GL_COLOR_BUFFER_BIT);
	Sleep(100);
	glFlush(); 	
}

void keyPressed(unsigned char key, int x, int y)
{
	pressed = key;
}

void Init()
{
   glClearColor(1.0,1.0,1.0,0.0);
   glColor3f(0.0f,0.0f,0.0f);
   glPointSize(4.0);
   glMatrixMode(GL_PROJECTION);
   glLoadIdentity();
   gluOrtho2D(0.0,screenWidth,0.0,screenHeight);
}

void main (int argc, char** argv)
{
	glutInit(&argc, argv);
	glutInitDisplayMode(GLUT_DOUBLE | GLUT_RGB);
	glutInitWindowSize(screenWidth, screenHeight);
	glutInitWindowPosition(100 , 150);
	glutCreateWindow("Main Game Loop");
	glutKeyboardFunc(keyPressed);
	glutDisplayFunc(Game);
	Init();
	glutMainLoop();
}
