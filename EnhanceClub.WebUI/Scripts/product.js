// typical import
import $ from 'jquery';
import gsap from 'gsap';
import ScrollTrigger from 'gsap/ScrollTrigger';
import PerfectScrollbar from 'perfect-scrollbar';

import { isMobile } from './common';

gsap.registerPlugin(ScrollTrigger);

const initAnimations = () => {

	const timelineProduct = gsap.timeline({
		scrollTrigger: {
			trigger: '.product-page-banner',
			start: 'top top',
			endTrigger: '.about-product',
			end: 'bottom bottom',
			scrub: true,
		},
	});

	timelineProduct.fromTo(
		document.querySelector('.product'),
		{ rotate: -30 },
		{ rotate: 18 }
	);

	const timelineProductPill = gsap.timeline({
		scrollTrigger: {
			trigger: '.product-page-banner',
			start: 'top top',
			endTrigger: '.about-product',
			end: 'center center',
			scrub: true,
		},
	});

	timelineProductPill.fromTo(
		document.querySelector('.product-pills'),
		{
			rotate: 75,
			right: 0,
		},
		{
			rotate: 160,
			right: '-35%',
		}
	);

	if (!isMobile()) {
		const timelineBanner = gsap.timeline({
			scrollTrigger: {
				trigger: '.product-page-banner',
				start: 'top top',
				endTrigger: '.about-product',
				end: 'center center',
				scrub: true,
			},
		});

		timelineBanner.fromTo(
			document.querySelector('.product-banner-bg'),
			{
				clipPath:
					'polygon(35% 0, 100% 0, 100% 76%, 100% 100%, 18% 100%, 82% 58%)',
			},
			{
				clipPath:
					'polygon(54.8625% 0px, 100% 0px, 100% 76%, 100% 100%, 45.1525% 100%, 82% 0%)',
			}
		);

		const timelineFormElements = gsap.timeline({
			scrollTrigger: {
				trigger: '.product-page-banner',
				start: 'top top',
				endTrigger: '.about-product',
				end: 'center center',
				scrub: true,
			},
		});

		timelineFormElements.fromTo(
			document.querySelector('.action-items .form-control'),
			{
				opacity: 1,
				left: '0',
			},
			{
				opacity: 1,
				left: '0',
			}
		);

		const timelineProdDescription = gsap.timeline({
			scrollTrigger: {
				trigger: '.about-product',
				start: 'top top',
				endTrigger: '.product-description',
				end: 'center center',
				scrub: true,
			},
		});

		timelineProdDescription.fromTo(
			document.querySelector(
				'.product-page .product-item .dose-recommendation .bg-box'
			),
			{
				opacity: 0,
				top: -130,
			},
			{
				opacity: 1,
				top: 0,
			}
		);

		const timelineProdDescriptionText = gsap.timeline({
			scrollTrigger: {
				trigger: '.about-product',
				start: 'top top',
				endTrigger: '.product-description',
				end: 'center center',
				scrub: true,
			},
		});

		timelineProdDescriptionText.fromTo(
			document.querySelector(
				'.product-page .product-item .dose-recommendation .dose-write-up'
			),
			{
				opacity: 0,
				y: 100,
			},
			{
				opacity: 1,
				y: 0,
			}
		);
	}
};

const initProduct = () => {

	if($('.product-page').length > 0) {
		initAnimations();

		if ($('#product-info').length > 0) {

			let ps = {};

			const initScrollbar = () => {
				ps = new PerfectScrollbar('#product-info .tab-content');
			};		

			if(window.innerWidth <= 768){
				ps.destroy();				
			}

			window.addEventListener('resize', () => {
				if(window.innerWidth <= 768){
					ps.destroy();
				} else {
					initScrollbar();
				}
			});
		
		}

		let topOffset = 0;
		let isOffsetSet = false;
		let bottomOffset = 20; // tweak here or get from margins etc

		const stickyRelocate = () => {

			if(isMobile()){
				return;
			}

			const windowTop = $(window).scrollTop();
			const windowHeight = $(window).height();
			const footerTop = $('#related-items').offset().top;
			const divTop = $('.product-item').offset().top;
			const divHeight = $('.product-item').height();

			if (isMobile() && !isOffsetSet) {
				if (divHeight < windowHeight) {
					topOffset = windowHeight - divHeight;
				}

				bottomOffset += topOffset;

				$('.product-item').css({
					marginTop: topOffset,
				});
			}

			if (windowTop + divHeight > footerTop - bottomOffset) {
				$('.product-item').css({
					top: (windowTop + divHeight - footerTop + bottomOffset) * -1,
				});
			} else if (windowTop > divTop) {
				$('.product-item').removeClass('is-sticky');
				$('.product-item').css({ top: 0 });
			} else {
				$('.product-item').addClass('is-sticky');
			}
		};

		$(window).on('scroll', stickyRelocate);

		$(document.body).on('touchmove', function(){
			isOffsetSet = true;
			stickyRelocate();
		});

		$(window).on('load', function() {
			if ($('.product-page').length) {
				stickyRelocate();						
			}
		});
	}
};

export default initProduct;


				}).fromTo(document.querySelector(".product-page .product-item .dose-recommendation .bg-box"), {
					opacity: 0,
					top: -130
				}, {
					opacity: 1,
					top: 0
				}), Ni.timeline({
					scrollTrigger: {
						trigger: ".about-product",
						start: "top top",
						endTrigger: ".product-description",
						end: "center center",
						scrub: !0
					}
				}).fromTo(document.querySelector(".product-page .product-item .dose-recommendation .dose-write-up"), {
					opacity: 0,
					y: 100
				}, {
					opacity: 1,
					y: 0
				}), r()("#product-info").length > 0) {
				var t = {};
				window.innerWidth <= 768 && t.destroy(), window.addEventListener("resize", (function() {
					window.innerWidth <= 768 ? t.destroy() : t = new oo("#product-info .tab-content")
				}))
			}
			var e = 0,
				n = !1,
				i = 20,
				o = function() {
					if (!so()) {
						var t = r()(window).scrollTop(),
							o = r()(window).height(),
							s = r()("#related-items").offset().top,
							a = r()(".product-item").offset().top,
							l = r()(".product-item").height();
						so() && !n && (l < o && (e = o - l), i += e, r()(".product-item").css({
							marginTop: e
						})), t + l > s - i ? r()(".product-item").css({
							top: -1 * (t + l - s + i)
						}) : t > a ? (r()(".product-item").removeClass("is-sticky"), r()(".product-item").css({
							top: 0
						})) : r()(".product-item").addClass("is-sticky")
					}
				};
			r()(window).on("scroll", o), r()(document.body).on("touchmove", (function() {
				n = !0, o()
			})), r()(window).on("load", (function() {
				r()(".product-page").length && o()
			}))
		}
	},
	lo = function() {
		r()(".tabs .tab-nav button").on("click", (function() {
			var t = r()(this).data("href"),
				e = r()(this).closest(".tabs").attr("id");
			r()(t).length && (r()(this).closest(".tabs").find(".tab-nav li").removeClass("active"), r()(this).closest(".tabs").find(".tab-content .tab").removeClass("active"), r()(this).parent("li").addClass("active"), r()(t).addClass("active"), e && r()(document).trigger("tab_updated", e))
		}))
	};
! function() {