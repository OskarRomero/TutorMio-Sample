import React, { Suspense, lazy, useState, useEffect } from "react";
import { createBrowserRouter, RouterProvider } from "react-router-dom";
import "bootstrap/dist/css/bootstrap.min.css";
import "bootstrap/dist/js/bootstrap.min.js";
import "./App.css";
import { jwtDecode } from "jwt-decode";
import RootLayout from "./pages/layouts/RootLayout";
// import AdminLayout from "./pages/layouts/AdminLayout";
import Landing from "./pages/landing/Landing";
import LogIn from "./pages/login/LogIn";
import { checkAuthLoader, tokenLoader } from "./services/auth";

const Test = lazy(() => import("../src/pages/pricing/Test"));
const Courses = lazy(() => import("./pages/courses/Courses"));
const NewCourse = lazy(() => import("./pages/courses/NewCourse"));
const ContactUs = lazy(() => import("./pages/contactus/ContactUs"));
const Pricing = lazy(() => import("./pages/pricing/Pricing"));
const PageNotFound = lazy(() => import("./pages/error/PageNotFound"));

function App() {
  const [currentUser, setCurrentUser] = useState({
    language: "spanish",
    firstName: "unknown",
    isLoggedIn: false,
    lastName: "User",
    avatarUrl: null,
    count: 0,
    wishlist: [],
    activeClass: null,
    activePlan: [],
    coursesData: {
      spanish: [],
      english: [],
      translation: {
        spanish: {},
        english: {},
      },
    },
    plansData: {
      spanish: [],
      english: [],
    },
  });
  useEffect(() => {
    const ifToken = localStorage.getItem("tutorMioToken");
    if (ifToken && !currentUser.isLoggedIn) {
      const decodedToken = jwtDecode(ifToken);
      const currentTime = Date.now() / 1000;
      decodedToken.exp > currentTime
        ? logInUser()
        : localStorage.removeItem("tutorMioToken");
    }
  }, [currentUser.isLoggedIn]);

  function logInUser() {
    setCurrentUser((prevState) => {
      let newState = { ...prevState };
      newState.isLoggedIn = true;
      return newState;
    });
  }

  function logOutUser() {
    localStorage.removeItem("tutorMioToken");
    setCurrentUser((prevState) => {
      let newState = { ...prevState };
      newState.isLoggedIn = false;
      return newState;
    });
  }

  function onClassRequested(data) {
    if (currentUser.wishlist.length === 0 && typeof data === "object") {
      wishlistPusher(data);
    } else if (currentUser.wishlist.length !== 0) {
      const incomingTitle = data.title;
      const duplicate = currentUser.wishlist.find(wishlistFinder);
      function wishlistFinder(item) {
        return item.title === incomingTitle;
      }
      duplicate ? console.log("duplicate class found") : wishlistPusher(data);
    }
  }

  function onRemoveFromWishlist(idx, qty, clearAll) {
    setCurrentUser((prevState) => {
      let newState = { ...prevState };
      newState.wishlist = [...prevState.wishlist];
      newState.wishlist.splice(idx, qty);
      if (clearAll === "clear-all") {
        newState.activePlan = [];
      }
      return newState;
    });
  }

  function wishlistPusher(courseClass) {
    setCurrentUser((prevState) => {
      let newState = { ...prevState };
      newState.wishlist = [...prevState.wishlist];
      newState.wishlist.push(courseClass);
      return newState;
    });
  }

  function setActiveClass(activeClass) {
    setCurrentUser((prevState) => {
      let newState = { ...prevState };
      newState.activeClass = activeClass;
      return newState;
    });
  }
  function onPlanChosen(data) {
    setCurrentUser((prevState) => {
      let newState = { ...prevState };
      newState.activePlan = [data];
      return newState;
    });
  }
  function onRemovePlan() {
    setCurrentUser((prevState) => {
      let newState = { ...prevState };
      newState.activePlan = [];
      return newState;
    });
  }

  function onRemovePlanFromWishlist() {
    setCurrentUser((prevState) => {
      let newState = { ...prevState };
      newState.activePlan = [];
      return newState;
    });
  }

  function setCourses(courses) {
    setCurrentUser((prevState) => {
      let newState = { ...prevState };
      newState.coursesData = { ...newState.coursesData };
      newState.coursesData[prevState.language] = courses;
      return newState;
    });
  }

  function setCourseTranslation(trans, lang) {
    setCurrentUser((prevState) => {
      let newState = { ...prevState };
      newState.coursesData = { ...newState.coursesData };
      newState.coursesData.translation[lang] = trans;
      return newState;
    });
  }

  function onGetPlans(plans) {
    setCurrentUser((prevState) => {
      let newState = { ...prevState };
      newState.plansData = { ...newState.plansData };
      newState.plansData[prevState.language] = plans;
      return newState;
    });
  }

  function onLanguageChange(language) {
    setCurrentUser((prevState) => {
      let newState = { ...prevState };
      newState.language = language;
      return newState;
    });
  }

  const router = createBrowserRouter([
    {
      path: "/",
      element: (
        <RootLayout
          user={currentUser}
          navigateToClass={setActiveClass}
          clearWishList={onRemoveFromWishlist}
          removeClass={onRemoveFromWishlist}
          removePlan={onRemovePlanFromWishlist}
          bubblingLang={onLanguageChange}
          logOutUser={logOutUser}
        />
      ),
      errorElement: <PageNotFound />,
      id: "root",
      loader: tokenLoader,
      children: [
        //landing
        {
          index: true,
          element: <Landing language={currentUser.language} />,
        },
        //courses
        {
          path: "courses",
          element: (
            <Courses
              notifyApp={onClassRequested}
              activeClass={currentUser.activeClass}
              wishlist={currentUser.wishlist}
              onDisactivateCurrentClass={setActiveClass}
              notifyAppRemove={onRemoveFromWishlist}
              courses={setCourses}
              tunnelingCourses={currentUser.coursesData}
              language={currentUser.language}
              bubblingCourseTranslation={setCourseTranslation}
              tunnelingCourseTranslation={currentUser.coursesData.translation}
            />
          ),
        },
        //courses/new
        {
          path: "courses/new",
          element: <NewCourse />,
          loader: checkAuthLoader,
        },
        //courses/new/:courseId
        {
          path: "courses/new/:courseId",
          element: <NewCourse />,
          loader: checkAuthLoader,
        },
        //contact
        {
          path: "contact",
          element: (
            <ContactUs
              activePlan={currentUser.activePlan}
              requestedClasses={currentUser.wishlist}
              language={currentUser.language}
            />
          ),
        },
        //plans
        {
          path: "plans",
          element: (
            <Pricing
              onPlanChosen={onPlanChosen}
              confirmedPlan={currentUser.activePlan}
              onRemovePlan={onRemovePlan}
              bubblingPlans={onGetPlans}
              tunnelingPlans={currentUser.plansData}
              language={currentUser.language}
            />
          ),
          // loader: checkAuthLoader,
        },
        //login
        {
          path: "login",
          element: (
            <LogIn
              logOutUser={logOutUser}
              isLoggedIn={currentUser.isLoggedIn}
              logInUser={logInUser}
            />
          ),
        },
        // test
        {
          path: "test",
          element: <Test />,
        },
      ],
    },
    //sample protected layout
    // {
    //   path: "/admin",
    //   element: (
    //     <AdminLayout
    //       user={currentUser}
    //       navigateToClass={setActiveClass}
    //       clearWishList={onRemoveFromWishlist}
    //       removeClass={onRemoveFromWishlist}
    //       removePlan={onRemovePlanFromWishlist}
    //       bubblingLang={onLanguageChange}
    //       updateUser={userLoggedIn}
    //     />
    //   ),
    //   errorElement: <PageNotFound />,
    //   children: [
    //     {
    //       index: true,
    //       element: <Landing language={currentUser.language} />,
    //     },
    //     {
    //       path: "login",
    //       element: <LogIn updateUser={currentUser.language} />,
    //     },
    //   ],
    // },
  ]);

  return (
    <Suspense fallback={<p style={{ position: "absolute" }}>loading...</p>}>
      {/* <ToastContainer /> */}
      <RouterProvider router={router} future={{ v7_startTransition: true }} />
    </Suspense>
  );
}

export default App;
