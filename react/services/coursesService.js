import axios from "axios";
import { getAuthToken } from "./auth";

const getTopCourses = () => {
  const token = getAuthToken();
  const config = {
    method: "GET",
    url: `${process.env.REACT_APP_API_URL}/courses/top`,
    withCredentials: true,
    crossdomain: true,
    headers: {
      "Content-Type": "application/json",
      Authorization: "Bearer " + token,
    },
  };
  return axios(config);
};

const getCoursesByLangv2 = (lang) => {
  const config = {
    method: "GET",
    url: `${process.env.REACT_APP_API_URL}/courses/language?language=${lang}`,
    withCredentials: false,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };
  return axios(config);
};

const coursesService = { getTopCourses, getCoursesByLangv2 };

export default coursesService;
